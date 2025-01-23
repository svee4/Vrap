using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class EnumEntry : FieldEntry, IDiscriminatedEntity<FieldType>
{
	public EnumOption Value { get; private set; } = null!;

	public static FieldType Discriminator => FieldType.Enum;

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
