namespace Vrap.LifeLog.Web.Features.DataTables.Table.Add;

public sealed class AddViewModel
{
	public required int TableId { get; init; }
	public required IReadOnlyList<FieldData> Fields { get; init; }
}
