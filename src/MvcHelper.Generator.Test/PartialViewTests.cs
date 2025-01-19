namespace MvcHelper.Generator.Test;

public class PartialViewTests
{
	[Fact]
	public void BasicPartialMapView()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapPartialView("./View")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.View();
}
""",
			path: @"C:\dev\Project\Features\Asd\MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);
		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var generatedSource = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace, ClassName)))
			.SourceText;

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Asd;

partial class MinimalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MinimalController controller)
	{
		private readonly MinimalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.PartialViewResult View() => 
	_controller.PartialView(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void MixedNormalAndPartial()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapPartialView("./Partial")]
[MapView("./View")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.View();
}
""",
			path: @"C:\dev\Project\Features\Asd\MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);
		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var generatedSource = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace, ClassName)))
			.SourceText;

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Asd;

partial class MinimalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MinimalController controller)
	{
		private readonly MinimalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.PartialViewResult Partial() => 
	_controller.PartialView(viewName: @"/Features/Asd/Partial.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void BasicPartialGeneric()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapPartialView<Model>("./Partial")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.Partial(new Model());
}

public record Model;
""",
			path: @"C:\dev\Project\Features\Asd\MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);
		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var generatedSource = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace, ClassName)))
			.SourceText;

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Asd;

partial class MinimalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MinimalController controller)
	{
		private readonly MinimalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.PartialViewResult Partial(global::Project.Features.Asd.Model model) => 
	_controller.PartialView(viewName: @"/Features/Asd/Partial.cshtml", model: model);
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

}
