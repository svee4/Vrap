using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Vrap.LifeLog.Web.Infra.Mvc;

public class MvcController : Controller
{
	public MvcResult Result => new(this);
}

public sealed class MvcResult(Controller controller)
{
	private readonly Controller _controller = controller;

	public IActionResult? Result { get; private set; }
	public HttpStatusCode? StatusCode { get; private set; }

	public MvcResult WithResult(IActionResult result)
	{
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

	public MvcResult WithStatus(HttpStatusCode status)
	{
		StatusCode = status;
		return this;
	}

	public IActionResult ToActionResult()
	{
		if (StatusCode is { } status)
		{
			_controller.HttpContext.Response.StatusCode = (int)status;
		}

		if (Result is { } result)
		{
			return result;
		}

		throw new InvalidOperationException("MvcResult.Result is not set");
	}
}
