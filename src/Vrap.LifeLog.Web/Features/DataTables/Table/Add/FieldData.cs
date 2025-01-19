using static Vrap.Database.LifeLog.Configuration.TableFieldHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Add;

public sealed class FieldData(int id, FieldType type, string name, FieldArguments args)
{
	public int Id { get; } = id;
	public FieldType Type { get; } = type;
	public string Name { get; } = name;

	public FieldArguments Arguments { get; } =
		args.Type == type
		? args
		: throw new InvalidOperationException("Mismatching type and args");
}
