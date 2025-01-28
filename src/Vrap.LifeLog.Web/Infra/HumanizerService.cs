using System.Globalization;
using Vrap.Database.LifeLog;
using Vrap.LifeLog.Web.Infra.RequestServices;

namespace Vrap.LifeLog.Web.Infra;

public sealed class HumanizerService(RequestTimeZone timeZone, RequestCulture culture)
{
	public const string DefaultDateFormat = "dd/MM/yyyy";
	public const string DefaultTimeFormat = "HH:mm:ss";
	public const string DefaultDateTimeFormat = $"{DefaultTimeFormat} {DefaultDateFormat} K";

	public CultureInfo Culture { get; } = culture.Culture ?? CultureInfo.InvariantCulture;
	public TimeZoneInfo TimeZone { get; } = timeZone.TimeZone ?? TimeZoneInfo.Utc;

	public string FieldValueSlimToString(LifeLogHelpers.FieldValueSlim value)
	{
		ArgumentNullException.ThrowIfNull(value);
		return LifeLogHelpers.MapFieldType(value.Type, value,
			(LifeLogHelpers.DateTimeValueSlim v) => ToDateTimeString(v.Value),
			(LifeLogHelpers.EnumValueSlim v) => v.Value,
			(LifeLogHelpers.NumberValueSlim v) => ToNumericString(v.Value),
			(LifeLogHelpers.StringValueSlim v) => v.Value
		);
	}

	private DateTimeOffset InRequestTimeZone(DateTimeOffset datetime) =>
		TimeZoneInfo.ConvertTime(datetime, TimeZone);

	public string ToDateString(DateTimeOffset datetime) =>
		InRequestTimeZone(datetime).ToString(DefaultDateFormat, Culture);

	public string ToTimeString(DateTimeOffset time) =>
		InRequestTimeZone(time).ToString(DefaultTimeFormat, Culture);

	public string ToDateTimeString(DateTimeOffset date) =>
		InRequestTimeZone(date).ToString(DefaultDateTimeFormat, Culture);

	public string ToNumericString(decimal value) =>
		value.ToString(Culture);

	public string ToJavascriptDateTime(DateTimeOffset datetime) =>
		InRequestTimeZone(datetime).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture);

	public string ToTimeZoneOffset(TimeZoneInfo tz)
	{
		var span = tz.BaseUtcOffset;
		var format = $"{(span.Ticks < 0 ? "'-'" : "'+'")}hh':'mm";
		return span.ToString(format, Culture);
	}
}
