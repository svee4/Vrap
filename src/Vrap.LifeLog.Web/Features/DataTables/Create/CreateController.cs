using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vrap.Database;
using Vrap.Database.LifeLog.Configuration;
using Vrap.LifeLog.Web.Infra.Mvc;

namespace Vrap.LifeLog.Web.Features.DataTables.Create;

[Route("DataTables/Create")]
[MvcHelper.MapView<CreateModel>("./CreateView")]
public partial class CreateController : MvcController
{
	[HttpGet("")]
	public IActionResult Get() => Views.CreateView(new CreateModel { Name = "" });

	[HttpPost("")]
	public async Task<IActionResult> Post(CreateModel model, [FromServices] VrapDbContext dbContext)
	{
		var name = model.Name;
		if (await dbContext.DataTables.AnyAsync(table => table.Name == name))
		{
			ModelState.AddModelError(nameof(CreateModel.Name), "Name is already in use");
			return Views.CreateView(model);
		}

		var table = DataTable.Create(name);
		_ = await dbContext.AddAsync(table);
		_ = await dbContext.SaveChangesAsync();

		return Result()
			.WithHtmxRedirect($"/DataTables/{table.Id}/Edit")
			.ToActionResult();
	}
}
