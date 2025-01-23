using System.Globalization;
using Vrap.Database.LifeLog;

namespace Vrap.LifeLog.Web.Infra;

// todo: make this more culture aware
public sealed class HumanizerService
{
	public const string DefaultDateFormat = "dd/MM/yyyy";
	public const string DefaultTimeFormat = "HH:mm:ss";
	public const string DefaultDateTimeFormat = $"{DefaultTimeFormat} {DefaultDateFormat}";



	public string FieldValueToString(LifeLogHelpers.FieldValueSlim value)
	{
		ArgumentNullException.ThrowIfNull(value);
		return LifeLogHelpers.MapFieldType(value.Type, value,
			(LifeLogHelpers.FieldValueSlim.DateTime v) => ToDateTimeString(v.Value),
			(LifeLogHelpers.FieldValueSlim.Enum v) => v.Value,
			(LifeLogHelpers.FieldValueSlim.Number v) => ToNumericString(v.Value),
			(LifeLogHelpers.FieldValueSlim.String v) => v.Value
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
