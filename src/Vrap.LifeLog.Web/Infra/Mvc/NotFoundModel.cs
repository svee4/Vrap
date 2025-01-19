namespace Vrap.LifeLog.Web.Infra.Mvc;

public sealed class NotFoundModel
{
	public required string RequestedPage { get; init; }
	public required string ReturnLink { get; init; }
}
