using System.Globalization;
using Vrap.Database.LifeLog.Configuration;
using static Vrap.Database.LifeLog.Configuration.TableFieldHelpers;

namespace Vrap.LifeLog.Web.Features.Data;

public static class DataHelpers
{
	public abstract record FieldEntry(FieldType Type, string Name)
	{
		public string Print() => MapFieldType(Type,
			dateTimeField: () => ((DateTimeEntry)this).Value.ToString("HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
			numberField: () => $"{((NumberEntry)this).Value}",
			stringField: () => $"{((StringEntry)this).Value}",
			enumField: () => $"{((EnumEntry)this).Value}"
		);

		public static FieldEntry FromDb(Database.LifeLog.Entries.FieldEntry entry) =>
			MapFieldType<FieldEntry>(GetFieldType(entry.TableField),
				dateTimeField: () => new DateTimeEntry(entry.TableField.Name, ((Database.LifeLog.Entries.DateTimeEntry)entry).Value),
				numberField: () => new NumberEntry(entry.TableField.Name, ((Database.LifeLog.Entries.NumberEntry)entry).Value),
				stringField: () => new StringEntry(entry.TableField.Name, ((Database.LifeLog.Entries.StringEntry)entry).Value),
				enumField: () => new EnumEntry(entry.TableField.Name, ((Database.LifeLog.Entries.EnumEntry)entry).Value)
			);
	}
	public sealed record DateTimeEntry(string Name, DateTimeOffset Value) : FieldEntry(FieldType.DateTime, Name);
	public sealed record EnumEntry(string Name, EnumOption Value) : FieldEntry(FieldType.Enum, Name);
	public sealed record NumberEntry(string Name, decimal Value) : FieldEntry(FieldType.Number, Name);
	public sealed record StringEntry(string Name, string Value) : FieldEntry(FieldType.String, Name);
}
