using System.Globalization;
using Vrap.Database.LifeLog;

namespace Vrap.LifeLog.Web.Infra;

// todo: make this more culture aware
public sealed class HumanizerService
{
	public const string DefaultDateFormat = "dd/MM/yyyy";
	public const string DefaultTimeFormat = "HH:mm:ss";
	public const string DefaultDateTimeFormat = $"{DefaultTimeFormat} {DefaultDateFormat}";

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


	public string ToDateString(DateTimeOffset datetime) =>
		datetime.ToString(DefaultDateFormat, CultureInfo.InvariantCulture);

	public string ToTimeString(DateTimeOffset datetime) =>
	datetime.ToString(DefaultTimeFormat, CultureInfo.InvariantCulture);

	public string ToDateTimeString(DateTimeOffset date) =>
		date.ToString(DefaultDateTimeFormat, CultureInfo.InvariantCulture);

	public string ToNumericString(decimal value) =>
		value.ToString(CultureInfo.InvariantCulture);
}
