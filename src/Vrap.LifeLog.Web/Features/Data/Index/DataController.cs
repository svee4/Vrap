using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcHelper;
using Vrap.Database;
using Vrap.LifeLog.Web.Infra.Mvc;

namespace Vrap.LifeLog.Web.Features.Data.Index;

[Route("Data")]
[MapView<DataViewModel>("./DataView")]
public sealed partial class DataController : MvcController
{

	[HttpGet("")]
	public async Task<IActionResult> Get([FromServices] VrapDbContext dbContext)
	{
		var latestEntries = await dbContext.DataEntries
			.OrderBy(entry => entry.Created)
			.Select(entry => new
			{
				TableId = entry.Table.Id,
				TableName = entry.Table.Name,
				EntryId = entry.Id,
				Created = entry.Created,
			})
			.Take(20)
			.ToListAsync();

		return Views.DataView(new DataViewModel
		{
			Entries = latestEntries
			.Select(entry => new EntryData(entry.TableId, entry.TableName, entry.EntryId, entry.Created))
			.ToList(),
		});
	}
}
