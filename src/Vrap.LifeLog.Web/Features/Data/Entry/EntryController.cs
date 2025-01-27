using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using Vrap.Database;
using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;
using Vrap.LifeLog.Web.Infra.Mvc;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.Data.Entry;

[Route("Data/Entry/{id:int}")]
[MapView<EntryViewModel>("./EntryView")]
public sealed partial class EntryController : MvcController
{
	[HttpGet("")]
	public async Task<IActionResult> Get(int id, [FromServices] VrapDbContext dbContext)
	{
		var entry = await dbContext.DataEntries
			.Where(entry => entry.Id == id)
			.Select(entry => new
			{
				TableName = entry.Table.Name,
				Created = entry.Created,
			})
			.SingleAsync();

		var fields = (await dbContext.FieldEntries
			.Where(field => field.Entry.Id == id)
			.Include(field => field.TableField)
			.ThenInclude(field => ((EnumField)field).Options)
			.OrderBy(field => field.TableField.Ordinal)
			.Select(field => new
			{
				Field = field,
				Type = field.TableField.FieldType,
				FieldName = field.TableField.Name
			})
			.ToListAsync())
			.Select(field => MapFieldType(field.Type, field.Field,
				FieldEntrySlim (DateTimeEntry v) => new DateTimeEntrySlim(field.FieldName, new DateTimeValueSlim(v.Value)),
				FieldEntrySlim (EnumEntry v) => new EnumEntrySlim(field.FieldName, new EnumValueSlim(v.Value.Value)),
				FieldEntrySlim (NumberEntry v) => new NumberEntrySlim(field.FieldName, new NumberValueSlim(v.Value)),
				FieldEntrySlim (StringEntry v) => new StringEntrySlim(field.FieldName, new StringValueSlim(v.Value))
			))
			.ToList();

		return Views.EntryView(new EntryViewModel
		{
			TableId = id,
			TableName = entry.TableName,
			Created = entry.Created,
			Fields = fields
		});
	}
}
