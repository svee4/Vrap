namespace Vrap.Database.LifeLog.Configuration;

public static class TableFieldHelpers
{
	public abstract record FieldArguments(FieldType Type, bool Required)
	{
		public static FieldArguments FromField(TableField field) =>
			MapFieldType<FieldArguments>(GetFieldType(field),
				dateTimeField: () => DateTimeArguments.FromField((DateTimeField)field),
				numberField: () => NumberArguments.FromField((NumberField)field),
				stringField: () => StringArguments.FromField((StringField)field),
				enumField: () => EnumArguments.FromField((EnumField)field)
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

		public sealed record Option(int Id, string Value);
	}


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
