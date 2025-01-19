using Microsoft.AspNetCore.Mvc;
using MvcHelper;

namespace Vrap.LifeLog.Web.Features.Index;

[MapView("./Index")]
public sealed partial class Index : Controller
{
	[HttpGet("")]
	public IActionResult Get() => Views.Index();
}
