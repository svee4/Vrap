namespace Vrap.Database.LifeLog.Configuration;

public sealed class NumberField : TableField
{
	public decimal? MinValue { get; private set; }
	public decimal? MaxValue { get; private set; }

	private NumberField() { }
	private NumberField(string name, bool required) : base(name, required) { }

	public static NumberField Create(string name, bool required, decimal? minValue, decimal? maxValue) =>
		new(name, required)
		{
			MinValue = minValue,
			MaxValue = maxValue
		};
}
