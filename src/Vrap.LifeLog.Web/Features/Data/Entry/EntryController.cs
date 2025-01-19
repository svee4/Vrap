using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using Vrap.Database;
using Vrap.Database.LifeLog.Configuration;
using Vrap.LifeLog.Web.Infra.Mvc;

namespace Vrap.LifeLog.Web.Features.Data.Entry;

[Route("Data/Entry/{id:int}")]
[MapView<EntryViewModel>("./EntryView")]
public sealed partial class EntryController : MvcController
{
	[HttpGet("")]
	public async Task<IActionResult> Get(int id, [FromServices] VrapDbContext dbContext)
	{
		var entry = await dbContext.DataEntries
			.AsNoTracking()
			.Where(entry => entry.Id == id)
			.Include(entry => entry.FieldEntries) // no sure if this is necessary
			.Select(entry => new
			{
				TableId = entry.Table.Id,
				TableName = entry.Table.Name,
				Created = entry.Created,
				FieldEntries = entry.FieldEntries
			})
			.FirstOrDefaultAsync();

		if (entry is null)
		{
			return Result.NotFound($"/Data/Entry/{id}", "/Data");
		}
		return null;
		//return Views.EntryView(new EntryViewModel
		//{
		//	TableId = entry.TableId,
		//	TableName = entry.TableName,
		//	Created = entry.Created,
		//	FieldEntries = entry.FieldEntries.Select(fe => 
		//		
		//		TableFieldHelpers.MapFieldType(TableFieldHelpers.GetFieldType(fe)))
		//});
	}
}
