using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcHelper;
using Vrap.LifeLog.Web.Infra.Mvc;

namespace Vrap.LifeLog.Web.Features.Auth.Login;

[Route("/Auth/Login")]
[AllowAnonymous]
[MapView("./LoginView")]
public sealed partial class LoginController : MvcController
{
	[HttpGet("")]
	public IActionResult Get([FromServices] SignInManager<IdentityUser> manager)
	{
		const string Provider = "Microsoft";
		var properties = manager.ConfigureExternalAuthenticationProperties(Provider, "/Auth/Callback");
		return new ChallengeResult(Provider, properties);
	}
}
