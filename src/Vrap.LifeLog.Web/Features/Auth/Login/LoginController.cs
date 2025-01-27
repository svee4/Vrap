using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vrap.LifeLog.Web.Infra.Mvc;

namespace Vrap.LifeLog.Web.Features.Auth.Login;

[Route("/Auth/Login")]
[AllowAnonymous]
public sealed partial class LoginController : MvcController
{
	[HttpGet("")]
	public IActionResult Get(
		[FromServices] SignInManager<IdentityUser> manager,
		[FromServices] ILogger<LoginController> logger)
	{
		const string Provider = "Microsoft";
		const string RedirectUri = "/Auth/Callback";

		logger.LogDebug("Request scheme: {Scheme}", Request.Scheme);
		var properties = manager.ConfigureExternalAuthenticationProperties(Provider, RedirectUri);
		return Challenge(properties, Provider);
	}
}
