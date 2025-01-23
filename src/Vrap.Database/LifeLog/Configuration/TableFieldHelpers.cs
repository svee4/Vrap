using Vrap.Database.LifeLog.Entries;

namespace Vrap.Database.LifeLog.Configuration;

[Obsolete("Use LifeLogHelpers")]
public static class TableFieldHelpers
{
	public abstract record FieldArguments(FieldType Type, bool Required, int Ordinal)
	{
		public static FieldArguments FromField(TableField field) =>
			MapFieldType<FieldArguments>(GetFieldType(field),
				dateTimeField: () => DateTimeArguments.FromField((DateTimeField)field),
				numberField: () => NumberArguments.FromField((NumberField)field),
				stringField: () => StringArguments.FromField((StringField)field),
				enumField: () => EnumArguments.FromField((EnumField)field)
			);
	}

	public sealed record DateTimeArguments(DateTimeOffset? MinValue, DateTimeOffset? MaxValue, bool Required, int Ordinal)
		: FieldArguments(FieldType.DateTime, Required, Ordinal)
	{
		public static DateTimeArguments FromField(DateTimeField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MinValue, field.MaxValue, field.Required, field.Ordinal);
		}
	}

	public sealed record NumberArguments(decimal? MinValue, decimal? MaxValue, bool Required, int Ordinal)
		: FieldArguments(FieldType.Number, Required, Ordinal)
	{
		public static NumberArguments FromField(NumberField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MinValue, field.MaxValue, field.Required, field.Ordinal);
		}
	}

	public sealed record StringArguments(int MaxLength, bool Required, int Ordinal) 
		: FieldArguments(FieldType.String, Required, Ordinal)
	{
		public static StringArguments FromField(StringField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.MaxLength, field.Required, field.Ordinal);
		}
	}

	public sealed record EnumArguments(IReadOnlyList<EnumArguments.Option> Options, bool Required, int Ordinal)
		: FieldArguments(FieldType.Enum, Required, Ordinal)
	{
		public static EnumArguments FromField(EnumField field)
		{
			ArgumentNullException.ThrowIfNull(field);
			return new(field.Options.Select(op => new Option(op.Id, op.Value)).ToList(), field.Required, field.Ordinal);
		}

		public sealed record Option(int Id, string Value);
	}

	public static FieldValue GetFieldValue(FieldEntry entry) => GetFieldType(entry.TableField) switch
	{
		FieldType.DateTime => new DateTimeValue(((DateTimeEntry)entry).Value),
		FieldType.Number => new NumberValue(((NumberEntry)entry).Value),
		FieldType.String => new StringValue(((StringEntry)entry).Value),
		FieldType.Enum => new EnumValue(((EnumEntry)entry).Value),
		_ => throw new InvalidOperationException("Unhandled field type"),
	};

	public abstract record FieldValue(FieldType Type);
	public sealed record DateTimeValue(DateTimeOffset Value) : FieldValue(FieldType.DateTime);
	public sealed record EnumValue(EnumOption Value) : FieldValue(FieldType.Enum);
	public sealed record NumberValue(decimal Value) : FieldValue(FieldType.Number);
	public sealed record StringValue(string Value) : FieldValue(FieldType.String);

	public static T MapFieldArguments<T>(
		FieldArguments arguments,
		Func<DateTimeArguments, T> dateTimeField,
		Func<NumberArguments, T> numberField,
		Func<StringArguments, T> stringField,
		Func<EnumArguments, T> enumField)
	{
		ArgumentNullException.ThrowIfNull(arguments);
		return MapFieldType(arguments.Type,
			() => dateTimeField((DateTimeArguments)arguments),
			() => numberField((NumberArguments)arguments),
			() => stringField((StringArguments)arguments),
			() => enumField((EnumArguments)arguments));
	}

	public static T MapFieldType<T>(
		FieldType fieldType,
		Func<T> dateTimeField,
		Func<T> numberField,
		Func<T> stringField,
		Func<T> enumField)
	{
		ArgumentNullException.ThrowIfNull(dateTimeField);
		ArgumentNullException.ThrowIfNull(numberField);
		ArgumentNullException.ThrowIfNull(stringField);
		ArgumentNullException.ThrowIfNull(enumField);

		return fieldType switch
		{
			FieldType.DateTime => dateTimeField(),
			FieldType.Number => numberField(),
			FieldType.String => stringField(),
			FieldType.Enum => enumField(),
			_ => throw new ArgumentException($"Unhandled fieldtype {fieldType}")
		};
	}

	public static T MapField<T>(
		TableField field,
		Func<DateTimeField, T> dateTimeField,
		Func<NumberField, T> numberField,
		Func<StringField, T> stringField,
		Func<EnumField, T> enumField)
	{
		ArgumentNullException.ThrowIfNull(field);
		ArgumentNullException.ThrowIfNull(dateTimeField);
		ArgumentNullException.ThrowIfNull(numberField);
		ArgumentNullException.ThrowIfNull(stringField);
		ArgumentNullException.ThrowIfNull(enumField);

		return GetFieldType(field) switch
		{
			FieldType.DateTime => dateTimeField((DateTimeField)field),
			FieldType.Number => numberField((NumberField)field),
			FieldType.String => stringField((StringField)field),
			FieldType.Enum => enumField((EnumField)field),
			_ => throw new ArgumentException($"Unhandled fieldtype {GetFieldType(field)}")
		};
	}

	public static FieldType GetFieldType(TableField field) => field switch
	{
		null => throw new ArgumentNullException(nameof(field)),
		DateTimeField => FieldType.DateTime,
		NumberField => FieldType.Number,
		StringField => FieldType.String,
		EnumField => FieldType.Enum,
		_ => throw new ArgumentException($"Unhandled fieldtype '{field.GetType().Name}'")
	};


	[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming",
		"CA1720:Identifier contains type name",
		Justification = "This isnt VB")]
	public enum FieldType
	{
		DateTime = 1,
		Number,
		String,
		Enum
	}

	public static IReadOnlyList<string> FieldTypes { get; } = Enum.GetNames<FieldType>();
}
