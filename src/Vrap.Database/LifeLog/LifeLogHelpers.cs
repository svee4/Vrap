using System.Diagnostics.CodeAnalysis;

namespace Vrap.Database.LifeLog;

public static partial class LifeLogHelpers
{

	[DoesNotReturn]
	internal static T ThrowUnsupportedFieldType<T>(string name) =>
		throw new ArgumentException($"Unknown FieldType '{name}' is not supported");

	public static IReadOnlyList<string> FieldTypes { get; } = Enum.GetNames<FieldType>();
}
