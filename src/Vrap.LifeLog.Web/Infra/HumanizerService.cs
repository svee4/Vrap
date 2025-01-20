using System.Globalization;

namespace Vrap.LifeLog.Web.Infra;

// todo: make this more culture aware
public sealed class HumanizerService
{
	public const string DefaultDateFormat = "dd/MM/yyyy";
	public const string DefaultTimeFormat = "HH:mm:ss";
	public const string DefaultDateTimeFormat = $"{DefaultTimeFormat} {DefaultDateFormat}";

	public string ToDateString(DateTimeOffset datetime) =>
		datetime.ToString(DefaultDateFormat, CultureInfo.InvariantCulture);

	public string ToTimeString(DateTimeOffset datetime) =>
	datetime.ToString(DefaultTimeFormat, CultureInfo.InvariantCulture);

	public string ToDateTimeString(DateTimeOffset date) =>
		date.ToString(DefaultDateTimeFormat, CultureInfo.InvariantCulture);
}
