using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class NumberEntry : FieldEntry
{
	public decimal Value { get; private set; }

	private NumberEntry() { }
	private NumberEntry(NumberField f, DataEntry? e) : base(f, e) { }

	public static NumberEntry Create(decimal value, NumberField field, DataEntry? entry) =>
		new(field, entry)
		{
			Value = value
		};
}
