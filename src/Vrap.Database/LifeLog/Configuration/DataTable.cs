using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vrap.Database.LifeLog.Entries;

namespace Vrap.Database.LifeLog.Configuration;

public sealed class DataTable : IEntityTypeConfiguration<DataTable>
{
	public const int NameMaxLength = 100;

	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public ICollection<TableField> Fields { get; private set; } = null!;
	public ICollection<DataEntry> Entries { get; private set; } = null!;

	private DataTable() { }

	public static DataTable Create(string name)
	{
		ArgumentException.ThrowIfNullOrEmpty(name);

		return new DataTable()
		{
			Name = name,
			Fields = [],
			Entries = []
		};
	}

	void IEntityTypeConfiguration<DataTable>.Configure(EntityTypeBuilder<DataTable> builder)
	{
		builder.Property(m => m.Name)
			.HasMaxLength(NameMaxLength);
	}
}
