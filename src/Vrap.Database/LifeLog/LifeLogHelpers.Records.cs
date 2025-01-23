using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;

namespace Vrap.Database.LifeLog;

static partial class LifeLogHelpers
{

	public abstract record FieldArguments
	{
		public FieldType Type { get; }
		public bool Required { get; }
		public int Ordinal { get; }

		private protected FieldArguments(FieldType type, bool required, int ordinal)
		{
			Type = type;
			Required = required;
			Ordinal = ordinal;
		}

		public static FieldArguments FromField(TableField field) =>
			MapFieldType(GetFieldType(field), field,
				FieldArguments (DateTimeField v) => new DateTimeArguments(v.MinValue, v.MaxValue, v.Required, v.Ordinal),
				FieldArguments (EnumField v) => new EnumArguments(v.Options.Select(op => new EnumArguments.Option(op.Id, op.Value)).ToList(), v.Required, v.Ordinal),
				FieldArguments (NumberField v) => new NumberArguments(v.MinValue, v.MaxValue, v.Required, v.Ordinal),
				FieldArguments (StringField v) => new StringArguments(v.MaxLength, v.Required, v.Ordinal)
			);
	}

	public sealed record DateTimeArguments(DateTimeOffset? MinValue, DateTimeOffset? MaxValue, bool Required, int Ordinal) 
		: FieldArguments(FieldType.DateTime, Required, Ordinal);

	public sealed record NumberArguments(decimal? MinValue, decimal? MaxValue, bool Required, int Ordinal) 
		: FieldArguments(FieldType.Number, Required, Ordinal);

	public sealed record StringArguments(int MaxLength, bool Required, int Ordinal)
		: FieldArguments(FieldType.String, Required, Ordinal);

	public sealed record EnumArguments(IReadOnlyList<EnumArguments.Option> Options, bool Required, int Ordinal)
		: FieldArguments(FieldType.Enum, Required, Ordinal)
	{
		public readonly record struct Option(int Id, string Value);
	}


	public abstract record FieldValueSlim
	{
		public FieldType Type { get; }
		private protected FieldValueSlim(FieldType type) => Type = type;
	}

	public sealed record DateTimeValueSlim(DateTimeOffset Value) : FieldValueSlim(FieldType.DateTime);
	public sealed record EnumValueSlim(string Value) : FieldValueSlim(FieldType.Enum);
	public sealed record NumberValueSlim(decimal Value) : FieldValueSlim(FieldType.Number);
	public sealed record StringValueSlim(string Value) : FieldValueSlim(FieldType.String);


	public abstract record FieldEntrySlim
	{
		public FieldType Type { get; }
		public string Name { get; init; }

		private protected FieldEntrySlim(FieldType type, string name) => (Type, Name) = (type, name);

		public FieldValueSlim GetValue() => MapFieldType(Type, this,
			static FieldValueSlim (DateTimeEntrySlim v) => v.Value,
			static FieldValueSlim (EnumEntrySlim v) => v.Value,
			static FieldValueSlim (NumberEntrySlim v) => v.Value,
			static FieldValueSlim (StringEntrySlim v) => v.Value
		);

		public static FieldEntrySlim FromFieldEntry(FieldEntry entry)
		{
			ArgumentNullException.ThrowIfNull(entry);
			return MapFieldType(GetFieldType(entry.TableField), entry,
				static FieldEntrySlim (DateTimeEntry v) => new DateTimeEntrySlim(v.TableField.Name, new(v.Value)),
				static FieldEntrySlim (EnumEntry v) => new EnumEntrySlim(v.TableField.Name, new(v.Value.Value)),
				static FieldEntrySlim (NumberEntry v) => new NumberEntrySlim(v.TableField.Name, new(v.Value)),
				static FieldEntrySlim (StringEntry v) => new StringEntrySlim(v.TableField.Name, new(v.Value))
			);
		}
	}

#pragma warning disable CA1721 // Property names should not match get methods

	public sealed record DateTimeEntrySlim(string Name, DateTimeValueSlim Value) : FieldEntrySlim(FieldType.DateTime, Name);
	public sealed record EnumEntrySlim(string Name, EnumValueSlim Value) : FieldEntrySlim(FieldType.Enum, Name);
	public sealed record NumberEntrySlim(string Name, NumberValueSlim Value) : FieldEntrySlim(FieldType.Number, Name);
	public sealed record StringEntrySlim(string Name, StringValueSlim Value) : FieldEntrySlim(FieldType.String, Name);

#pragma warning restore CA1721 // Property names should not match get methods


}
