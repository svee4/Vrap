using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class EnumEntry : FieldEntry
{
	public EnumOption Value { get; private set; } = null!;

	private EnumEntry() { }
	private EnumEntry(EnumField f, DataEntry? e) : base(f, e) { }

	public static EnumEntry Create(EnumOption value, EnumField field, DataEntry? entry)
	{
		ArgumentNullException.ThrowIfNull(value);
		return new(field, entry)
		{
			Value = value
		};
	}
}
