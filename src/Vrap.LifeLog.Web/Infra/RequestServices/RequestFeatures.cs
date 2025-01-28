using Microsoft.AspNetCore.Http.Features;
using System.Collections;

namespace Vrap.LifeLog.Web.Infra.RequestServices;

public sealed class RequestFeatures(IHttpContextAccessor accessor) : IFeatureCollection
{
	private readonly HttpContext _context = accessor.HttpContext
		?? throw new ArgumentException("Could not get HttpContext from accessor");

	private IFeatureCollection Features => _context.Features;

	public object? this[Type key] { get => Features[key]; set => Features[key] = value; }
	public bool IsReadOnly => Features.IsReadOnly;
	public int Revision => Features.Revision;
	public TFeature? Get<TFeature>() => Features.Get<TFeature>();
	public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => Features.GetEnumerator();
	public void Set<TFeature>(TFeature? instance) => Features.Set(instance);
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Features).GetEnumerator();
}
