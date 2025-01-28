namespace Vrap.LifeLog.Web.Infra.RequestServices;

public interface ITimeZoneFeature
{
	TimeZoneInfo TimeZone { get; }
}

public sealed class TimeZoneFeature(TimeZoneInfo timeZone) : ITimeZoneFeature
{
	public TimeZoneInfo TimeZone { get; } = timeZone;
}
