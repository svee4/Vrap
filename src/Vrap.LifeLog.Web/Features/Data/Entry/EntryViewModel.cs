using Vrap.Database.LifeLog.Configuration;
using static Vrap.Database.LifeLog.Configuration.TableFieldHelpers;

namespace Vrap.LifeLog.Web.Features.Data.Entry;

public sealed class EntryViewModel
{
	public required int TableId { get; init; }
	public required string TableName { get; init; }
	public required DateTimeOffset Created { get; init; }
	public required IReadOnlyList<FieldEntry> FieldEntries { get; init; }
}

public abstract record FieldEntry(FieldType Type, string Name);
public sealed record DateTimeEntry(string Name, DateTimeOffset Value) : FieldEntry(FieldType.DateTime, Name);
public sealed record EnumEntry(string Name, EnumOption Value) : FieldEntry(FieldType.Enum, Name);
public sealed record NumberEntry(string Name, decimal Value) : FieldEntry(FieldType.Number, Name);
public sealed record StringEntry(string Name, string Value) : FieldEntry(FieldType.String, Name);
