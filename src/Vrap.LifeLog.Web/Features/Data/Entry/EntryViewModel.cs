using Vrap.Database.LifeLog;

namespace Vrap.LifeLog.Web.Features.Data.Entry;

public sealed class EntryViewModel
{
	public required int TableId { get; init; }
	public required string TableName { get; init; }
	public required DateTimeOffset Created { get; init; }
	public required IReadOnlyList<LifeLogHelpers.FieldEntrySlim> Fields { get; init; }
}
