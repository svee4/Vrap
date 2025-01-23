using Vrap.Database.LifeLog.Configuration;
using Vrap.Database.LifeLog.Entries;

namespace Vrap.Database.LifeLog;

static partial class LifeLogHelpers
{

	public abstract record FieldArguments
	{
		public FieldType Type { get; }
		public bool Required { get; }

		private protected FieldArguments(FieldType type, bool required) => (Type, Required) = (type, required);

		public static FieldArguments FromField(TableField field) =>
			MapFieldType(GetFieldType(field), field,
				FieldArguments (DateTimeField v) => DateTimeArguments.FromField(v),
				FieldArguments (EnumField v) => EnumArguments.FromField(v),
				FieldArguments (NumberField v) => NumberArguments.FromField(v),
				FieldArguments (StringField v) => StringArguments.FromField(v)
			);
	}

	public sealed record DateTimeArguments(DateTimeOffset? MinValue, DateTimeOffset? MaxValue, bool Required)
		: FieldArguments(FieldType.DateTime, Required)
	{
		public static DateTimeArguments FromField(DateTimeField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MinValue, field.MaxValue, field.Required);
		}
	}

	public sealed record NumberArguments(decimal? MinValue, decimal? MaxValue, bool Required)
		: FieldArguments(FieldType.Number, Required)
	{
		public static NumberArguments FromField(NumberField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MinValue, field.MaxValue, field.Required);
		}
	}

	public sealed record StringArguments(int MaxLength, bool Required) : FieldArguments(FieldType.String, Required)
	{
		public static StringArguments FromField(StringField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MaxLength, field.Required);
		}
	}


	public sealed record EnumArguments(IReadOnlyList<EnumArguments.Option> Options, bool Required)
		: FieldArguments(FieldType.Enum, Required)
	{
		public static EnumArguments FromField(EnumField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.Options.Select(op => new Option(op.Id, op.Value)).ToList(), field.Required);
		}

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
