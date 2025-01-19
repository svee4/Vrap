namespace Vrap.Database.LifeLog.Configuration;

public sealed class StringField : TableField
{
	public const int AbsoluteMaxLength = 1000;

	public int MaxLength
	{
		get;
		private set
		{
			// not sure how i feel about doing this validation here but ill let it be for now
			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, AbsoluteMaxLength);
			ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0);
			field = value;
		}
	}

	private StringField() { }
	private StringField(string name, bool required) : base(name, required) { }

	public static StringField Create(string name, bool required, int maxLength) => 
		new(name, required)
		{
			MaxLength = maxLength
		};
}
