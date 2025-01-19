using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class StringEntry : FieldEntry, IEntityTypeConfiguration<StringEntry>
{
	public string Value { get; private set; } = null!;

	private StringEntry() { }
	private StringEntry(StringField f, DataEntry? e) : base(f, e) { }

	public static StringEntry Create(string value, StringField field, DataEntry? entry)
	{
		ArgumentNullException.ThrowIfNull(value);
		return new(field, entry)
		{
			Value = value
		};
	}

	void IEntityTypeConfiguration<StringEntry>.Configure(EntityTypeBuilder<StringEntry> builder)
	{
		builder.Property(m => m.Value)
			.HasMaxLength(StringField.AbsoluteMaxLength);
	}
}
