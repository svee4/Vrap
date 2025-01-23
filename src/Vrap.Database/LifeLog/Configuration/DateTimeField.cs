namespace Vrap.Database.LifeLog.Configuration;

public sealed class DateTimeField : TableField, IDiscriminatedEntity<FieldType>
{
	public DateTimeOffset? MinValue { get; private set => field = value?.ToUniversalTime(); }
	public DateTimeOffset? MaxValue { get; private set => field = value?.ToUniversalTime(); }

	public static FieldType Discriminator => FieldType.DateTime;

	private DateTimeField() { }
	private DateTimeField(string name, bool required, int ordinal) : base(name, required, ordinal) { }

	public static DateTimeField Create(string name, bool required, int ordinal, DateTimeOffset? minValue, DateTimeOffset? maxValue) =>
		new(name, required, ordinal)
		{
			MinValue = minValue,
			MaxValue = maxValue
		};
}
