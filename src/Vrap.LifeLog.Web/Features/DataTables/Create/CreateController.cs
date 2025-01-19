using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vrap.Database;
using Vrap.Database.LifeLog.Configuration;

namespace Vrap.LifeLog.Web.Features.DataTables.Create;

[Route("DataTables/Create")]
[MvcHelper.MapView<CreateViewModel>("./CreateView")]
public partial class CreateController : Controller
{
	[HttpGet("")]
	public IActionResult Get() => Views.CreateView(new CreateViewModel { Name = "" });

	[HttpPost("")]
	public async Task<IActionResult> Post(CreateViewModel model, [FromServices] VrapDbContext dbContext)
	{
		var name = model.Name;
		if (await dbContext.DataTables.AnyAsync(table => table.Name == name))
		{
			ModelState.AddModelError(nameof(CreateViewModel.Name), "Name is already in use");
			return Views.CreateView(model);
		}

		var table = DataTable.Create(name);
		_ = await dbContext.AddAsync(table);
		_ = await dbContext.SaveChangesAsync();

		return Redirect($"/DataTables/{table.Id}/Edit");
	}
}
