namespace Vrap.Database.LifeLog.Configuration;

public sealed class EnumField : TableField, IDiscriminatedEntity<FieldType>
{
	/// <summary>
	/// THIS IS NOT ENFORCED BY THE DATABASE
	/// </summary>
	public const int OptionMaxLength = 50;

	public ICollection<EnumOption> Options { get; private set; } = null!;

	public static FieldType Discriminator => FieldType.Enum;


	private EnumField() { }
	private EnumField(string name, bool required, int ordinal) : base(name, required, ordinal) { }

	public static EnumField Create(string name, bool required, int ordinal, IEnumerable<EnumOption>? options) =>
		new EnumField(name, required, ordinal)
		{
			Options = options is null ? [] : [.. options]
		};
}
