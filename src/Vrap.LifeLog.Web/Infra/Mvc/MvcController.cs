using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;

namespace Vrap.LifeLog.Web.Infra.Mvc;

public abstract class MvcController : Controller
{
	public MvcResult Result() => new(this);
}

public sealed class MvcResult(Controller controller)
{
	private readonly Controller _controller = controller;

	public IActionResult? Result { get; private set; }
	public HttpStatusCode? StatusCode { get; private set; }
	public string? Redirect { get; private set; }

	private static void EnsurePropNotSet<T>(T? prop, [CallerArgumentExpression(nameof(prop))] string propName = "")
	{
		if (prop is not null)
		{
			ThrowAlreadySet(propName);
		}
	}

	[DoesNotReturn]
	private static void ThrowAlreadySet(string propName) => throw new InvalidOperationException($"{propName} is already set");

	public MvcResult WithResult(IActionResult result)
	{
		EnsurePropNotSet(Result);
		Result = result;
		return this;
	}

	public MvcResult WithView(ViewResult view) => WithResult(view);

	public MvcResult WithView(PartialViewResult partialView) => WithResult(partialView);

	public IActionResult NotFound(string requestedPage, string returnLink) =>
		WithResult(_controller.View("/Infra/Mvc/NotFound.cshtml", model: new NotFoundModel
		{
			RequestedPage = requestedPage,
			ReturnLink = returnLink
		}))
		.WithStatus(HttpStatusCode.NotFound)
		.ToActionResult();

	public IActionResult ServerError(string message) =>
		WithResult(_controller.Content(message))
		.WithStatus(HttpStatusCode.InternalServerError)
		.ToActionResult();

	public MvcResult WithStatus(HttpStatusCode status)
	{
		EnsurePropNotSet(StatusCode);
		StatusCode = status;
		return this;
	}

	public MvcResult WithRedirect(string to)
	{
		EnsurePropNotSet(Redirect);
		Redirect = to;
		return WithResult(_controller.Redirect(to));
	}

	public MvcResult WithHtmxRedirect(string to)
	{
		EnsurePropNotSet(Redirect);
		Redirect = to;
		return WithResult(_controller.Ok());
	}

	public IActionResult ToActionResult()
	{
		if (Result is not { } result)
		{
			throw new InvalidOperationException("MvcResult.Result is not set");
		}

		if (StatusCode is { } status)
		{
			_controller.Response.StatusCode = (int)status;
		}

		if (Redirect is { } redirect)
		{
			_controller.Response.Headers.Add("HX-Redirect", redirect);
		}

		return result;
	}
}
