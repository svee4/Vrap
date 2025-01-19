using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vrap.Database.LifeLog.Configuration;

public sealed class EnumOption
{
	public const int ValueMaxLength = 50;

	public int Id { get; private set; }
	public string Value { get; private set; } = null!;
	public EnumField Field { get; private set; } = null!;

	private EnumOption() { }

	public static EnumOption Create(string value, EnumField? field)
	{
		ArgumentNullException.ThrowIfNull(value);

		return new()
		{
			Value = value,
			Field = field!
		};
	}

	internal sealed class EnumOptionConfiguration : IEntityTypeConfiguration<EnumOption>
	{
		public void Configure(EntityTypeBuilder<EnumOption> builder)
		{
			builder.Property(m => m.Value)
				.HasMaxLength(ValueMaxLength);
		}
	}
}
