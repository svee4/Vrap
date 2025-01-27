using System.Globalization;
using Vrap.Database.LifeLog;
using static Vrap.Database.LifeLog.LifeLogHelpers;

namespace Vrap.LifeLog.Web.Features.DataTables.Table.Edit.Partials;

public sealed class FieldPartialModel(string name, FieldType type, FieldArguments arguments, bool isHtmxRequest)
{
	public string Name { get; } = name;
	public FieldType Type { get; } = type;
	public FieldArguments Arguments { get; } = arguments.Type == type
		? arguments
		: throw new ArgumentException("Mismatching Type and FieldArguments");

	public bool IsHtmxRequest { get; } = isHtmxRequest;

	public IReadOnlyList<string> PrintArguments()
	{
		var sections = MapFieldType(Arguments.Type, Arguments,
			static IReadOnlyList<string> (DateTimeArguments args) => [
				$"Minimum value: {args.MinValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}",
				$"Maximum value: {args.MaxValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}"],
			static (EnumArguments args) => [$"Options ({args.Options.Count}): {string.Join(", ", args.Options.Select(op => op.Value))}"],
			static (NumberArguments args) => [
				$"Minimum value: {args.MinValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}",
				$"Maximum value: {args.MaxValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}"],
			static (StringArguments _) => []
		);

		return [$"Required: {Arguments.Required}", .. sections];
	}
}
