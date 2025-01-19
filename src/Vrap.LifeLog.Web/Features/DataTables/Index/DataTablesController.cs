using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vrap.Database;

namespace Vrap.LifeLog.Web.Features.DataTables.Index;

[Route("DataTables")]
[MvcHelper.MapView<DataTablesViewModel>("./DataTablesView")]
public partial class DataTablesController : Controller
{
	[HttpGet("")]
	public async Task<IActionResult> Get([FromServices] VrapDbContext dbContext)
	{
		var tables = await dbContext.DataTables
			.Select(table => new DataTablesViewModel.Table
			{
				Id = table.Id,
				Name = table.Name,
			})
			.ToListAsync();

		return Views.DataTablesView(new DataTablesViewModel { Tables = tables });
	}
}
