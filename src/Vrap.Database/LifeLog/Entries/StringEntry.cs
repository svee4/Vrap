using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class StringEntry : FieldEntry
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

	internal sealed class StringEntryConfiguration : IEntityTypeConfiguration<StringEntry>

	{
		public void Configure(EntityTypeBuilder<StringEntry> builder)
		{
			builder.Property(m => m.Value)
				.HasMaxLength(StringField.AbsoluteMaxLength);
		}
	}
}
