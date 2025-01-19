using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vrap.Database.LifeLog.Configuration;

public sealed class EnumField : TableField
{
	public const int OptionMaxLength = 50;

	public ICollection<EnumOption> Options { get; private set; } = null!;

	private EnumField() { }
	private EnumField(string name, bool required) : base(name, required) { }

	public static EnumField Create(string name, bool required, IEnumerable<EnumOption>? options) =>
		new EnumField(name, required)
		{
			Options = options is null ? [] : [.. options]
		};

	internal sealed class EnumFieldConfiguration : IEntityTypeConfiguration<EnumField>
	{
		public void Configure(EntityTypeBuilder<EnumField> builder)
		{
			builder.Property(m => m.Options)
				.HasMaxLength(OptionMaxLength);
		}
	}
}
