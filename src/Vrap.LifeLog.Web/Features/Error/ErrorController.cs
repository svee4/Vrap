using Microsoft.AspNetCore.Mvc;
using MvcHelper;
using System.Diagnostics;

namespace Vrap.LifeLog.Web.Features.Error;

[Route("/Error")]
[MapView<ErrorModel>("./ErrorView")]
public sealed partial class ErrorController : Controller
{
	[HttpGet("")]
	public IActionResult Get() => Views.ErrorView(new ErrorModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
}

public sealed record ErrorModel(string RequestId);
