using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using System.Runtime.InteropServices;
using Vrap.Database;
using Vrap.Database.LifeLog;
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

		var fields = await dbContext.FieldEntries
			.Where(field => field.Entry.Id == id)
			.Select(field => new
			{
				Field = field,
				Type = 
			})
			.ToListAsync();

		if (entry is null)
		{
			return Result.NotFound($"/Data/Entry/{id}", "/Data");
		}

		return null;

		//return Views.EntryView(new EntryViewModel
		//{
		//	TableId = id,
		//	TableName = entry.TableName,
		//	Created = entry.Created,
		//	Fields = entry.Fields.All().ToList()
		//});
	}
}
