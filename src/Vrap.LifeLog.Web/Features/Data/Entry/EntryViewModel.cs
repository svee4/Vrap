namespace Vrap.LifeLog.Web.Features.Data.Entry;

public sealed class EntryViewModel
{
	public required int TableId { get; init; }
	public required string TableName { get; init; }
	public required DateTimeOffset Created { get; init; }
	public required IReadOnlyList<string> Headers { get; init; }
	public required IReadOnlyList<string> Fields { get; init; }
}
