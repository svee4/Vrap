using Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit;

public sealed class EditViewModel
{
	public required int Id { get; init; }
	public required DataTable Table { get; init; }

	public sealed class DataTable
	{
		public required string Name { get; init; }
		public required IReadOnlyList<FieldPartialModel> Fields { get; init; }
	}
}
