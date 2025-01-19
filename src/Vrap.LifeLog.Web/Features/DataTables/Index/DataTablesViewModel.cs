namespace Vrap.LifeLog.Web.Features.DataTables.Index;

public sealed class DataTablesViewModel
{
	public required IReadOnlyList<Table> Tables { get; init; }

	public sealed class Table
	{
		public required int Id { get; init; }
		public required string Name { get; init; }
	}
}
