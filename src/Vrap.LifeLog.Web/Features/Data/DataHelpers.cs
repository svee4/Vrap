using System.Globalization;
using Vrap.Database.LifeLog;
using Vrap.Database.LifeLog.Configuration;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.Data;

public static class DataHelpers
{
	public abstract record FieldEntry(FieldType Type, string Name)
	{
		public string Print() => MapFieldType(Type,
			() => ((DateTimeEntry)this).Value.ToString("HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture),
			() => $"{((EnumEntry)this).Value}",
			() => $"{((NumberEntry)this).Value}",
			() => $"{((StringEntry)this).Value}"
		);

		public static FieldEntry FromDb(Vrap.Database.LifeLog.Entries.FieldEntry entry) =>
			MapFieldType<FieldEntry>(GetFieldType(entry.TableField),
				() => new DateTimeEntry(entry.TableField.Name, ((Vrap.Database.LifeLog.Entries.DateTimeEntry)entry).Value),
				() => new EnumEntry(entry.TableField.Name, ((Vrap.Database.LifeLog.Entries.EnumEntry)entry).Value),
				() => new NumberEntry(entry.TableField.Name, ((Vrap.Database.LifeLog.Entries.NumberEntry)entry).Value),
				() => new StringEntry(entry.TableField.Name, ((Vrap.Database.LifeLog.Entries.StringEntry)entry).Value)
			);
	}
	public sealed record DateTimeEntry(string Name, DateTimeOffset Value) : FieldEntry(FieldType.DateTime, Name);
	public sealed record EnumEntry(string Name, EnumOption Value) : FieldEntry(FieldType.Enum, Name);
	public sealed record NumberEntry(string Name, decimal Value) : FieldEntry(FieldType.Number, Name);
	public sealed record StringEntry(string Name, string Value) : FieldEntry(FieldType.String, Name);
}
