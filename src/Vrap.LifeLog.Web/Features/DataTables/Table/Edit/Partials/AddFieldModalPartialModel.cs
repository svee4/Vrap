using Vrap.Database.LifeLog.Configuration;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;

public sealed record AddFieldModalPartialModel
{
	public required string Name { get; init; }
	public required int TableId { get; init; }
	public required TableFieldHelpers.FieldType? Type { get; init; }
	public required bool Required { get; init; }
	public required IReadOnlyList<string> FieldTypes { get; init; }

	private TableFieldHelpers.FieldArguments? _args;
	public required TableFieldHelpers.FieldArguments? FieldArguments
	{
		get => _args;
		init => _args =
			value is null || value.Type == Type
				? value
				: throw new ArgumentException("Mismatching field and argument types");
	}
}
