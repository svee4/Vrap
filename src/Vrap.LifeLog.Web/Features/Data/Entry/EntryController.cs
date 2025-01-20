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
			.Where(entry => entry.Id == id)
			.Select(entry => new
			{
				TableName = entry.Table.Name,
				Created = entry.Created,
				Headers = entry.FieldEntries.Select(field => field.TableField.Name).ToList(),
				Fields = entry.FieldEntries.Select(field => TableFieldHelpers.GetFieldValue(field)).ToList()
			})
			.SingleAsync();

		if (entry is null)
		{
			return Result.NotFound($"/Data/Entry/{id}", "/Data");
		}

		return null!;
	}
}
