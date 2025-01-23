using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using System.Text.RegularExpressions;
using Vrap.Database;
using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;
using Vrap.LifeLog.Web.Features.DataTables.Table.Add;
using Vrap.LifeLog.Web.Infra.Mvc;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table;

[Route("DataTables/{id:int}/Add")]
[MapView<AddViewModel>("./AddView")]
[MapPartialView<AddViewModel>("./AddView", Name = "AddViewPartial")]
public sealed partial class AddController : MvcController
{
	[HttpGet("")]
	public async Task<IActionResult> GetAdd(int id, [FromServices] VrapDbContext dbContext)
	{
		var fields = await GetTableFields(id, dbContext);
		return fields is null
			? Result().NotFound($"/DataTables/{id}/Add", "/DataTables")
			: Views.AddView(new AddViewModel()
			{
				TableId = id,
				TableName = (await dbContext.DataTables.SingleAsync(table => table.Id == id)).Name,
				Fields = MapFieldDatas(fields)
			});
	}

	[HttpPost("")]
	public async Task<IActionResult> Add(int id, [FromServices] VrapDbContext dbContext, CancellationToken token)
	{
		var fields = await GetTableFields(id, dbContext);
		if (fields is null)
		{
			return Result().NotFound($"/DataTables/{id}/Add", "/DataTables");
		}

		var helper = new ParsingHelper(id, dbContext, this);

		var success = true;
		foreach (var field in fields)
		{
			var key = $"field-{field.Id}";
			var formValue = Request.Form[key];

			var args = FieldArguments.FromField(field);

			if (formValue.Count == 0)
			{
				if (args.Required)
				{
					ModelState.AddModelError(key, "Field is required");
					success = false;
					break;
				}
				else
				{
					continue;
				}
			}

			if (formValue.Count > 1)
			{
				ModelState.AddModelError(key, "Cannot have more than one value");
				success = false;
				break;
			}

			var value = formValue[0];

			if (value is null)
			{
				ModelState.AddModelError(key, "Value cannot be null");
				success = false;
				break;
			}

			success = await helper.TryParseAndAdd(field, key, value, args, token);
			if (!success)
			{
				break;
			}
		}

		if (!success)
		{
			return Result().WithView(
				Views.AddViewPartial(new AddViewModel()
				{
					Fields = MapFieldDatas(fields),
					TableName = (await dbContext.DataTables.SingleAsync(table => table.Id == id, token)).Name,
					TableId = id
				}))
				.WithStatus(System.Net.HttpStatusCode.BadRequest)
				.ToActionResult();
		}

		var fieldEntries = helper.Entries;
		var table = await dbContext.DataTables.FirstOrDefaultAsync(table => table.Id == id, token);

		var entry = DataEntry.Create(DateTimeOffset.Now, table, fieldEntries);
		_ = dbContext.DataEntries.Add(entry);
		_ = await dbContext.SaveChangesAsync(token);

		return Result().WithHtmxRedirect($"/Data/Entry/{entry.Id}").ToActionResult();
	}

	private sealed partial class ParsingHelper(int tableId, VrapDbContext dbContext, Controller controller)
	{
		private readonly int _tableId = tableId;
		private readonly VrapDbContext _dbContext = dbContext;
		private readonly Controller _controller = controller;
		private ModelStateDictionary ModelState => _controller.ModelState;

		public List<FieldEntry> Entries { get; } = [];

		public async Task<bool> TryParseAndAdd(TableField field, string key, string value, FieldArguments args, CancellationToken token) =>
			await MapFieldType(GetFieldType(field),
				() => TryParseDateTime(key, value, (DateTimeField)field, (DateTimeArguments)args),
				() => TryParseEnum(key, value, (EnumField)field, (EnumArguments)args, token),
				() => TryParseNumber(key, value, (NumberField)field, (NumberArguments)args),
				() => TryParseString(key, value, (StringField)field, (StringArguments)args)
			);

		private Task<bool> TryParseDateTime(string fieldName, string value, DateTimeField field, DateTimeArguments args)
		{
			if (!DateTime.TryParse(value, out var date))
			{
				ModelState.AddModelError(fieldName, "Could not parse value");
				return Task.FromResult(false);
			}

			if (!int.TryParse(_controller.Request.Form["timezone"], out var timezone))
			{
				ModelState.AddModelError(fieldName, "Could not parse timezone");
				return Task.FromResult(false);
			}

			DateTimeOffset timeValue;
			try
			{
				var timezoneOffset = TimeSpan.FromMinutes(timezone);
				timeValue = new DateTimeOffset(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, timezoneOffset);
			}
			catch (ArgumentException)
			{
				return Task.FromResult(false);
			}

			if (args.MinValue is { } minValue && timeValue < minValue)
			{
				ModelState.AddModelError(fieldName, "Value too small");
				return Task.FromResult(false);
			}

			if (args.MaxValue is { } maxValue && timeValue > maxValue)
			{
				ModelState.AddModelError(fieldName, "Value too big");
				return Task.FromResult(false);
			}

			Entries.Add(DateTimeEntry.Create(timeValue, field, null));
			return Task.FromResult(true);
		}

		private Task<bool> TryParseNumber(string fieldName, string value, NumberField field, NumberArguments args)
		{
			if (!decimal.TryParse(value, out var numValue))
			{
				ModelState.AddModelError(fieldName, "Could not parse value");
				return Task.FromResult(false);
			}

			if (args.MinValue is { } minValue && numValue < minValue)
			{
				ModelState.AddModelError(fieldName, "Value too small");
				return Task.FromResult(false);
			}

			if (args.MaxValue is { } maxValue && numValue > maxValue)
			{
				ModelState.AddModelError(fieldName, "Value too big");
				return Task.FromResult(false);
			}

			Entries.Add(NumberEntry.Create(numValue, field, null));
			return Task.FromResult(true);
		}

		private Task<bool> TryParseString(string fieldName, string value, StringField field, StringArguments args)
		{
			if (value.Length > args.MaxLength)
			{
				ModelState.AddModelError(fieldName, "Value too long");
				return Task.FromResult(false);
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				ModelState.AddModelError(fieldName, "Value cannot be empty or only whitespace");
				return Task.FromResult(false);
			}

			value = value.Trim();

			try
			{
				value = MoreThanOneWhitespaceRegex().Replace(value, " ");
			}
			catch (RegexMatchTimeoutException)
			{
				ModelState.AddModelError(fieldName, "Invalid value");
				return Task.FromResult(false);
			}

			Entries.Add(StringEntry.Create(value, field, null));
			return Task.FromResult(true);
		}

		private async Task<bool> TryParseEnum(string fieldName, string value, EnumField field, EnumArguments args, CancellationToken token)
		{
			if (!int.TryParse(value, out var optionId))
			{
				ModelState.AddModelError(fieldName, "Invalid value");
				return false;
			}

			if (!args.Options.Any(op => op.Id == optionId))
			{
				ModelState.AddModelError(fieldName, "Invalid value");
				return false;
			}

			var option = await _dbContext.EnumOptions
				.Where(option => option.Field.Table.Id == _tableId)
				.Where(option => option.Id == optionId)
				.FirstOrDefaultAsync(token);

			if (option is null)
			{
				ModelState.AddModelError(fieldName, "Invalid option");
				return false;
			}

			Entries.Add(EnumEntry.Create(option, field, null));
			return true;
		}

		[GeneratedRegex(@"\s{2,}", RegexOptions.None, matchTimeoutMilliseconds: 100)]
		private static partial Regex MoreThanOneWhitespaceRegex();
	}

	private static async Task<ICollection<TableField>?> GetTableFields(int tableId, VrapDbContext dbContext)
	{
		var tempFields = await dbContext
			.DataTables
			.Where(table => table.Id == tableId)
			.Select(table => table.Fields.OrderBy(field => field.Ordinal).ToList())
			.FirstOrDefaultAsync();

		return tempFields;
	}

	private static List<FieldData> MapFieldDatas(IEnumerable<TableField> fields)
	{

		var fields2 = fields
			.Select(field => new FieldData(
				id: field.Id,
				type: GetFieldType(field),
				name: field.Name,
				args: FieldArguments.FromField(field)))
			.ToList();

		return fields2;
	}

}
