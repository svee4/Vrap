namespace Vrap.Database.LifeLog.Configuration;

public sealed class DateTimeField : TableField
{
	public DateTimeOffset? MinValue { get; private set => field = value?.ToUniversalTime(); }
	public DateTimeOffset? MaxValue { get; private set => field = value?.ToUniversalTime(); }

	private DateTimeField() { }
	private DateTimeField(string name, bool required) : base(name, required) { }

	public static DateTimeField Create(string name, bool required, DateTimeOffset? minValue, DateTimeOffset? maxValue) =>
		new(name, required)
		{
			MinValue = minValue,
			MaxValue = maxValue
		};
}
