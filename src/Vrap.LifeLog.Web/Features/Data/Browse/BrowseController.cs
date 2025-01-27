using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using Vrap.Database;
using Vrap.Database.LifeLog;
using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;
using Vrap.LifeLog.Web.Infra.Mvc;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.Data.Browse;

[Route("Data/Browse")]
[MapView<BrowseViewModel>("./BrowseView")]
public sealed partial class BrowseController : MvcController
{
	[HttpGet("{id:int}")]
	public async Task<IActionResult> Get(int id, [FromServices] VrapDbContext dbContext)
	{
		var tableData = await dbContext.DataTables
			.Where(table => table.Id == id)
			.Include(table => table.Fields)
			.ThenInclude(field => ((EnumField)field).Options)
			.Select(table => new
			{
				Name = table.Name,
				Fields = table.Fields
					.OrderBy(field => field.Ordinal)
					.Select(field => new
					{
						Type = field.FieldType,
						Name = field.Name
					})
					.ToList()
			})
			.FirstOrDefaultAsync();

		if (tableData is null)
		{
			return Result().NotFound("1", "2");
		}

		var entries = await dbContext.DataEntries
			.Where(entry => entry.Table.Id == id)
			.Include(entry => entry.FieldEntries)
				.ThenInclude(field => field.TableField)
			.Include(entry => entry.FieldEntries)
				.ThenInclude(field => ((EnumField)field.TableField).Options)
			.Select(entry => new
			{
				Id = entry.Id,
				TableName = entry.Table.Name,
				Created = entry.Created,
				Fields = entry.FieldEntries.OrderBy(field => field.TableField.Ordinal).ToList()
			})
			.Take(20)
			.ToListAsync();

		var fields = tableData.Fields.Select(field => new FieldData(field.Type, field.Name)).ToList();

		var entries2 = entries.Select(entry => new EntryData(entry.Id, entry.Created, entry.Fields.Select(ValueSelector).ToList()));

		return Views.BrowseView(new BrowseViewModel
		{
			Fields = fields,
			Entries = entries2.ToList(),
			TableId = id,
			TableName = tableData.Name
		});

		static FieldValueSlim ValueSelector(FieldEntry entry) => MapFieldType(entry.FieldType, entry,
				static FieldValueSlim (DateTimeEntry e) => new DateTimeValueSlim(e.Value),
				static FieldValueSlim (EnumEntry e) => new EnumValueSlim(e.Value.Value),
				static FieldValueSlim (NumberEntry e) => new NumberValueSlim(e.Value),
				static FieldValueSlim (StringEntry e) => new StringValueSlim(e.Value)
			);
	}
}
