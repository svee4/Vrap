using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vrap.Database.LifeLog.Configuration;

public abstract class TableField 
{
	public const int NameMaxLength = 100;

	public int Id { get; private set; }
	public string Name { get; set; }
	public bool Required { get; set; }
	public int Ordinal { get; set; }
	public DataTable Table { get; private set; } = null!;

	public FieldType FieldType { get; private set; }

	// ef core
	protected TableField() => Name = null!;

	protected TableField(string name, bool required, int ordinal)
	{
		ArgumentNullException.ThrowIfNull(name);
		Name = name;
		Required = required;
		Ordinal = ordinal;
	}	

	internal sealed class TableFieldConfiguration : IEntityTypeConfiguration<TableField>
	{
		public void Configure(EntityTypeBuilder<TableField> builder)
		{
			builder.Property(m => m.Name)
				.HasMaxLength(NameMaxLength);

			builder.HasDiscriminator<FieldType>(m => m.FieldType);
		}
	}
}
