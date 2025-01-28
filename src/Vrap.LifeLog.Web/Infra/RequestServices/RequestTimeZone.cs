namespace Vrap.LifeLog.Web.Infra.RequestServices;

public sealed class RequestTimeZone(RequestFeatures features)
{
	public TimeZoneInfo? TimeZone { get; } = features.Get<ITimeZoneFeature>()?.TimeZone;
}
