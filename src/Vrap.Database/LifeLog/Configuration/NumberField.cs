namespace Vrap.Database.LifeLog.Configuration;

public sealed class NumberField : TableField, IDiscriminatedChild<FieldType>
{
	public decimal? MinValue { get; private set; }
	public decimal? MaxValue { get; private set; }

	public static FieldType Discriminator => FieldType.Number;

	private NumberField() { }
	private NumberField(string name, bool required, int ordinal) : base(name, required, ordinal) { }

	public static NumberField Create(string name, bool required, int ordinal, decimal? minValue, decimal? maxValue) =>
		new(name, required, ordinal)
		{
			MinValue = minValue,
			MaxValue = maxValue
		};
}
