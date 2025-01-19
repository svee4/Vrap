namespace Vrap.Shared;

public static class EnumerableExtensions
{
	public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source) =>
		source.ToList().AsReadOnly();
}
