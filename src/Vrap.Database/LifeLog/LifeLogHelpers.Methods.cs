using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog;

public static partial class LifeLogHelpers
{

	public static TResult MapFieldType<TBase, TDateTime, TEnum, TNumber, TString, TResult>(
			FieldType fieldType,
			TBase value,
			Func<TDateTime, TResult> dateTimeSelector,
			Func<TEnum, TResult> enumSelector,
			Func<TNumber, TResult> numberSelector,
			Func<TString, TResult> stringSelector)
		where TBase : notnull
		where TDateTime : notnull, TBase
		where TEnum : notnull, TBase
		where TNumber : notnull, TBase
		where TString : notnull, TBase
	{
		ArgumentNullException.ThrowIfNull(value);
		ArgumentNullException.ThrowIfNull(dateTimeSelector);
		ArgumentNullException.ThrowIfNull(enumSelector);
		ArgumentNullException.ThrowIfNull(numberSelector);
		ArgumentNullException.ThrowIfNull(stringSelector);

		return fieldType switch
		{
			FieldType.DateTime => dateTimeSelector((TDateTime)value),
			FieldType.Enum => enumSelector((TEnum)value),
			FieldType.Number => numberSelector((TNumber)value),
			FieldType.String => stringSelector((TString)value),
			_ => ThrowUnsupportedFieldType<TResult>(fieldType.ToString())
		};
	}

	public static TResult MapFieldType<TResult>(
		FieldType fieldType,
		Func<TResult> dateTimeSelector,
		Func<TResult> enumSelector,
		Func<TResult> numberSelector,
		Func<TResult> stringSelector)
	{
		ArgumentNullException.ThrowIfNull(dateTimeSelector);
		ArgumentNullException.ThrowIfNull(enumSelector);
		ArgumentNullException.ThrowIfNull(numberSelector);
		ArgumentNullException.ThrowIfNull(stringSelector);

		return fieldType switch
		{
			FieldType.DateTime => dateTimeSelector(),
			FieldType.Enum => enumSelector(),
			FieldType.Number => numberSelector(),
			FieldType.String => stringSelector(),
			_ => ThrowUnsupportedFieldType<TResult>(fieldType.ToString())
		};
	}

	//public static Expression<Func<TableField, TResult>> MapFieldTypeExpr<TResult>(
	//	Func<TResult> dateTimeSelector
	//	Func<TResult> enumSelector
	//	Func<TResult> numberSelector
	//	Func<TResult> stringSelector
	//) => field => 



	public static FieldType GetFieldType(TableField field) => field switch
	{
		null => throw new ArgumentNullException(nameof(field)),
		DateTimeField => FieldType.DateTime,
		NumberField => FieldType.Number,
		StringField => FieldType.String,
		EnumField => FieldType.Enum,
		_ => throw new ArgumentException($"Unhandled field type '{field.GetType().Name}'")
	};

}
