using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class DateTimeEntry : FieldEntry
{
	public DateTimeOffset Value { get; private set; }

	private DateTimeEntry() { }
	private DateTimeEntry(DateTimeField f, DataEntry? e) : base(f, e) { }

	public static DateTimeEntry Create(DateTimeOffset value, DateTimeField field, DataEntry? entry) =>
		new(field, entry)
		{
			Value = value.ToUniversalTime()
		};
}
