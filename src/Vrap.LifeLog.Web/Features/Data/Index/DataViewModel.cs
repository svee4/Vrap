namespace Vrap.LifeLog.Web.Features.Data.Index;

public sealed class DataViewModel
{
	public required IReadOnlyList<EntryData> Entries { get; init; }
}

public sealed record EntryData(int TableId, string TableName, int EntryId, DateTimeOffset Created);
