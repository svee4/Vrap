using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Vrap.LifeLog.Web.Infra.Mvc;

/// <summary>
/// Allows you to use relative paths in a relative tag helper, 
/// ie <code>&lt;relative-partial name="Partials/MyPartial.cshtml" /&gt;</code>
/// translates to
/// <code>&lt;partial name="/Absolute/View/Directory/Partials/MyPartial.cshtml" /&gt;</code>
/// </summary>
/// <param name="viewEngine"></param>
/// <param name="viewBufferScope"></param>
public sealed class RelativePartialTagHelper(
		ICompositeViewEngine viewEngine,
		IViewBufferScope viewBufferScope)
	: PartialTagHelper(viewEngine, viewBufferScope)
{
	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		var thisPath = ViewContext.ExecutingFilePath
			?? throw new InvalidOperationException("null ViewContext.ExecutingFilePath");

		thisPath = Normalize(thisPath);
		var directory = Normalize(Path.GetDirectoryName(thisPath) ?? "/");
		var absoluteTagPath = Normalize(Path.Combine(directory, Name));

		if (Path.GetExtension(absoluteTagPath) is null or "")
		{
			absoluteTagPath = Normalize(Path.ChangeExtension(absoluteTagPath, "cshtml"));
		}

		Name = absoluteTagPath;
		await base.ProcessAsync(context, output);

		static string Normalize(string path) => path.Replace('\\', '/');
	}
}
