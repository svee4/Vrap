using Vrap.Database.LifeLog;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;

public sealed record AddFieldModalPartialModel
{
	public required string Name { get; init; }
	public required int TableId { get; init; }
	public required FieldType? Type { get; init; }
	public required bool Required { get; init; }
	public required int Ordinal { get; init; }
	public required IReadOnlyList<string> FieldTypes { get; init; }

	private FieldArguments? _args;
	public required FieldArguments? FieldArguments
	{
		get => _args;
		init => _args =
			value is null || value.Type == Type
				? value
				: throw new ArgumentException("Mismatching field and argument types");
	}
}
