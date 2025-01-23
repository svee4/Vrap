using Vrap.Database.LifeLog;

namespace Vrap.LifeLog.Web.Features.Data.Browse;

public sealed class BrowseViewModel
{
	public required int TableId { get; init; }
	public required string TableName { get; init; }
	public required IReadOnlyList<FieldData> Fields { get; init; }
	public required IReadOnlyList<EntryData> Entries { get; init; }
}

public record FieldData(FieldType Type, string FieldName);
public record EntryData(int Id, DateTimeOffset Created, IReadOnlyList<DataHelpers.FieldEntry> Entries);
