using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog.Entries;

public abstract class FieldEntry
{
	public int Id { get; private set; }
	public DataEntry Entry { get; private set; }
	public TableField TableField { get; private set; }

	public FieldType FieldType { get; private set; }

	// ef core
	protected FieldEntry()
	{
		Entry = null!;
		TableField = null!;
	}

	protected FieldEntry(TableField tableField, DataEntry? entry)
	{
		ArgumentNullException.ThrowIfNull(tableField);
		TableField = tableField;
		Entry = entry!;
	}
}
