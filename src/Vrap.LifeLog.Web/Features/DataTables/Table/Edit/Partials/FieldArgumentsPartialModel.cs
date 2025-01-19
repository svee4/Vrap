using static Vrap.Database.LifeLog.Configuration.TableFieldHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;

public sealed class FieldArgumentsPartialModel(
	FieldType fieldType,
	FieldArguments? arguments)
{
	public FieldType FieldType { get; init; } = fieldType;
	public FieldArguments? FieldArguments { get; init; } =
		arguments is null || arguments.Type == fieldType
			? arguments
			: throw new ArgumentException("Mismatching field arguments");
}
