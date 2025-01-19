using System.Globalization;
using static Vrap.Database.LifeLog.Configuration.TableFieldHelpers;

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
		var sections = MapFieldArguments<IReadOnlyList<string>>(Arguments,
			dateTimeField: args => [
				$"Minimum value: {args.MinValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}",
				$"Maximum value: {args.MaxValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}"],
			numberField: args => [
				$"Minimum value: {args.MinValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}",
				$"Maximum value: {args.MaxValue?.ToString(CultureInfo.InvariantCulture) ?? "null"}"],
			stringField: _ => [],
			enumField: args => [$"Options ({args.Options.Count}): {string.Join(", ", args.Options)}"]
		);

		return sections;
	}
}
