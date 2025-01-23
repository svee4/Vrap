using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using Vrap.Database;
using Vrap.Database.LifeLog;
using Vrap.LifeLog.Web.Infra.Mvc;

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
			.Select(table => new
			{
				Name = table.Name,
				Fields = table.Fields
					.OrderBy(field => field.Name)
					.Select(field => new
					{
						Name = field.Name,
						Type = LifeLogHelpers.GetFieldType(field)
					})
			})
			.FirstOrDefaultAsync();

		if (tableData is null)
		{
			return Result().NotFound("1", "2");
		}

		var query = dbContext.DataEntries
			.Where(entry => entry.Table.Id == id)
			.Include(entry => entry.FieldEntries)
				.ThenInclude(field => field.TableField)
			.Select(entry => new
			{
				Id = entry.Id,
				TableName = entry.Table.Name,
				Created = entry.Created,
				Fields = entry.FieldEntries.OrderBy(field => field.TableField.Name).ToList()
			})
			.Take(100);

		var results = await query.ToListAsync();

		return Views.BrowseView(new BrowseViewModel
		{
			Fields = tableData.Fields.Select(field => new FieldData(field.Type, field.Name)).ToList(),
			Entries = results.Select(
				row => new EntryData(row.Id, row.Created, row.Fields.Select(DataHelpers.FieldEntry.FromDb).ToList()))
			.ToList(),
			TableId = id,
			TableName = tableData.Name
		});
	}
}
