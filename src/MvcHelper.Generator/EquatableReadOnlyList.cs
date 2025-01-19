using System.Collections;

namespace MvcHelper.Generator;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

internal sealed class EquatableReadOnlyList<T>
	: IEquatable<EquatableReadOnlyList<T>>, IReadOnlyList<T> where T : IEquatable<T>
{
	private readonly IReadOnlyList<T> _list;

	private EquatableReadOnlyList(IReadOnlyList<T> list) =>
		_list = list;

	public static EquatableReadOnlyList<T> CreateCopy(IEnumerable<T> source) =>
		new(source.ToArray());

	public static EquatableReadOnlyList<T> CreateCopy(ReadOnlySpan<T> source) =>
		new(source.ToArray());

	public static EquatableReadOnlyList<T> CreateMove(IReadOnlyList<T> source) =>
		new(source);

	public T this[int index] => _list[index];

	public int Count => _list.Count;

	public bool Equals(EquatableReadOnlyList<T> other) => _list.SequenceEqual(other._list);
	public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public override bool Equals(object obj) =>
		obj is EquatableReadOnlyList<T> other && Equals(other);
}

#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

internal static class EquatableReadOnlyListExtensions
{
	public static EquatableReadOnlyList<T> CopyToEquatableReadOnlyList<T>(this IEnumerable<T> source)
			where T : IEquatable<T> =>
		EquatableReadOnlyList<T>.CreateCopy(source);

	public static EquatableReadOnlyList<T> CopyToEquatableReadOnlyList<T>(this Span<T> source)
			where T : IEquatable<T> =>
		EquatableReadOnlyList<T>.CreateCopy(source);

	public static EquatableReadOnlyList<T> CopyToEquatableReadOnlyList<T>(this ReadOnlySpan<T> source)
			where T : IEquatable<T> =>
		EquatableReadOnlyList<T>.CreateCopy(source);

	public static EquatableReadOnlyList<T> MoveToEquatableReadOnlyList<T>(this IReadOnlyList<T> source)
			where T : IEquatable<T> =>
		EquatableReadOnlyList<T>.CreateMove(source);

}
