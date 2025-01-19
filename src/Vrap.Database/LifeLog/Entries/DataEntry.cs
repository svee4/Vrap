using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public sealed class DataEntry
{
	public int Id { get; private set; }
	public DateTimeOffset Created { get; private set; }
	public ICollection<FieldEntry> FieldEntries { get; private set; } = null!;
	public DataTable Table { get; private set; } = null!;

	private DataEntry() { }

	public static DataEntry Create(DateTimeOffset created, DataTable? table, IEnumerable<FieldEntry>? fieldEntries) =>
		new()
		{
			Created = created.ToUniversalTime(),
			Table = table!,
			FieldEntries = fieldEntries is null ? [] : [.. fieldEntries]
		};
}
