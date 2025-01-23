using System.Diagnostics.CodeAnalysis;
using LinqKit;
using System.Linq.Expressions;
using Vrap.Database.LifeLog.Entries;
using Vrap.Database.LifeLog.Configuration;

namespace Vrap.Database.LifeLog;

public static partial class LifeLogHelpers
{

	[DoesNotReturn]
	internal static T ThrowUnsupportedFieldType<T>(string name) =>
		throw new ArgumentException($"Unknown FieldType '{name}' is not supported");

	public static IReadOnlyList<string> FieldTypes { get; } = Enum.GetNames<FieldType>();
}
