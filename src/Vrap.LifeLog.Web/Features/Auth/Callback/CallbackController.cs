using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vrap.LifeLog.Web.Infra.Mvc;
using Vrap.Shared;

namespace Vrap.LifeLog.Web.Features.Auth.Callback;

[Route("/Auth/Callback")]
[AllowAnonymous]
public sealed partial class CallbackController : MvcController
{
	[HttpGet("")]
	public async Task<IActionResult> Index(string? returnUrl, string? remoteError,
		[FromServices] SignInManager<IdentityUser> signinManager,
		[FromServices] UserManager<IdentityUser> userManager,
		[FromServices] IConfiguration configuration,
		[FromServices] ILogger<CallbackController> logger)
	{

		returnUrl ??= "/";
		if (remoteError != null)
		{
			return Result().ServerError("Remote error");
		}

		var info = await signinManager.GetExternalLoginInfoAsync();
		if (info is null)
		{
			return Unauthorized("No external login info");
		}

		var microsoftUserId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
		var email = info.Principal.FindFirstValue(ClaimTypes.Email);

		if (microsoftUserId is null || email is null)
		{
			return Unauthorized("Invalid claims");
		}

		var allowedMicrosoftUserIds = configuration.GetRequiredConfiguration("Vrap:LifeLog:Auth:Microsoft:AllowedUserIds").Split(",");
		if (!allowedMicrosoftUserIds.Contains(microsoftUserId))
		{
			return Unauthorized("Disallowed account");
		}

		logger.LogInformation("Authenticating user {Email}", email);

		var result = await signinManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

		if (!result.Succeeded)
		{
			if (result.IsLockedOut || result.IsNotAllowed || result.RequiresTwoFactor)
			{
				throw new ServerException("Failed to sign in");
			}

			var user = new IdentityUser
			{
				UserName = microsoftUserId,
				Email = email
			};

			var result2 = await userManager.CreateAsync(user);
			if (!result2.Succeeded)
			{
				throw new ServerException($"Failed to create account: {string.Join(", ", result2.Errors)}");
			}

			var result3 = await userManager.AddLoginAsync(user, info);
			if (!result3.Succeeded)
			{
				throw new ServerException($"Failed to add login: {string.Join(", ", result3.Errors)}");
			}

			await signinManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
		}

		return LocalRedirect(returnUrl);
	}
}
