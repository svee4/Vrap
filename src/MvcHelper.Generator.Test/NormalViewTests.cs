namespace MvcHelper.Generator.Test;

public class NormalViewTests
{
	[Fact]
	public void BasicMapView()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

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
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void TwoMapViews()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("./View")]
[MapView("./Error")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.View();
	public IActionResult Lol() => Views.Error();
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
public global::Microsoft.AspNetCore.Mvc.ViewResult Error() => 
	_controller.View(viewName: @"/Features/Asd/Error.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void ExplicitViewPathExtension()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("./View.cshtml")]
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
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void AbsoluteViewPath()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("/Views/MinimalView")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.MinimalView();
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
public global::Microsoft.AspNetCore.Mvc.ViewResult MinimalView() => 
	_controller.View(viewName: @"/Views/MinimalView.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void OneGenericMapView()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView<MyViewData>("/Views/MinimalView")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.MinimalView(new MyViewData());
}

public sealed record MyViewData;
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
public global::Microsoft.AspNetCore.Mvc.ViewResult MinimalView(global::Project.Features.Asd.MyViewData model) => 
	_controller.View(viewName: @"/Views/MinimalView.cshtml", model: model);
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void MixedMapViews()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView<MyViewData>("/Views/MinimalView")]
[MapView("/Views/Error")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.MinimalView(new MyViewData());
	public IActionResult Lol() => Views.Error();
}

public sealed record MyViewData;
""",
			path: @"C:\dev\Project\Features\Asd\MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);

		Assert.Equal(2, generatorResult.GeneratedSources.Length);

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
public global::Microsoft.AspNetCore.Mvc.ViewResult Error() => 
	_controller.View(viewName: @"/Views/Error.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult MinimalView(global::Project.Features.Asd.MyViewData model) => 
	_controller.View(viewName: @"/Views/MinimalView.cshtml", model: model);
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}


	[Fact]
	public void ManyMixedMapViews()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView<MyViewData1>("/Views/View1")]
[MapView<MyViewData2>("/Views/View2")]
[MapView("/Views/Error1")]
[MapView("/Views/Error2")]
public sealed partial class MinimalController : Controller
{
	public IActionResult One() => Views.View1(new MyViewData1());
	public IActionResult Two() => Views.View2(new MyViewData2());
	public IActionResult Three() => Views.Error1();
	public IActionResult Four() => Views.Error2();
}

public sealed record MyViewData1;
public sealed class MyViewData2;
""",
			path: @"C:\dev\Project\Features\Asd\MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);

		Assert.Equal(2, generatorResult.GeneratedSources.Length);

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
public global::Microsoft.AspNetCore.Mvc.ViewResult Error1() => 
	_controller.View(viewName: @"/Views/Error1.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult Error2() => 
	_controller.View(viewName: @"/Views/Error2.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View1(global::Project.Features.Asd.MyViewData1 model) => 
	_controller.View(viewName: @"/Views/View1.cshtml", model: model);
public global::Microsoft.AspNetCore.Mvc.ViewResult View2(global::Project.Features.Asd.MyViewData2 model) => 
	_controller.View(viewName: @"/Views/View2.cshtml", model: model);
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void TwoClassesSameNamespace()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName1 = "MinimalController";
		const string ClassName2 = "MaximalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("/Views/View")]
public sealed partial class MinimalController : Controller
{
	public IActionResult One() => Views.View();
}

[MapView("/Views/View")]
[MapView("/Views/View2")]
public sealed partial class MaximalController : Controller
{
	public IActionResult One() => Views.View();
	public IActionResult Two() => Views.View2();
}
""",
			path: @"C:\dev\Project\Features\Asd\Controllers.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);

		Assert.Equal(3, generatorResult.GeneratedSources.Length);

		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var minimalSource = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace, ClassName1)))
			.SourceText;

		var maximalSource = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace, ClassName2)))
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
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
	}
}

""",
			actual: minimalSource.ToString(),
			ignoreLineEndingDifferences: true);

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Asd;

partial class MaximalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MaximalController controller)
	{
		private readonly MaximalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View2() => 
	_controller.View(viewName: @"/Views/View2.cshtml");
	}
}

""",
			actual: maximalSource.ToString(),
			ignoreLineEndingDifferences: true);
	}

	[Fact]
	public void TwoClassesDifferentNamespaces()
	{
		const string Namespace1 = "Project.Features.Asd";
		const string Namespace2 = "Project.Features.Xyz";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd
{
	[MapView("/Views/View")]
	public sealed partial class MinimalController : Controller
	{
		public IActionResult One() => Views.View();
	}
}

namespace Project.Features.Xyz
{
	[MapView("/Views/View")]
	[MapView("/Views/View2")]
	public sealed partial class MinimalController : Controller
	{
		public IActionResult One() => Views.View();
		public IActionResult Two() => Views.View2();
	}
}
""",
			path: @"C:\dev\Project\Features\Asd\Controllers.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source));

		var generatorResult = Assert.Single(result.Results);

		Assert.Equal(3, generatorResult.GeneratedSources.Length);

		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var result1 = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace1, ClassName)))
			.SourceText;

		var result2 = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace2, ClassName)))
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
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
	}
}

""",
			actual: result1.ToString(),
			ignoreLineEndingDifferences: true);

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Xyz;

partial class MinimalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MinimalController controller)
	{
		private readonly MinimalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View2() => 
	_controller.View(viewName: @"/Views/View2.cshtml");
	}
}

""",
			actual: result2.ToString(),
			ignoreLineEndingDifferences: true);
	}


	[Fact]
	public void TwoClassesDifferentFiles()
	{
		const string Namespace1 = "Project.Features.Asd";
		const string Namespace2 = "Project.Features.Xyz";
		const string ClassName = "MinimalController";

		var source1 = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd
{
	[MapView("/Views/View")]
	public sealed partial class MinimalController : Controller
	{
		public IActionResult One() => Views.View();
	}
}
""",
			path: @"C:\dev\Project\Features\Asd\Controllers.cs");

		var source2 = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

namespace Project.Features.Xyz
{
	[MapView("/Views/View")]
	[MapView("/Views/View2")]
	public sealed partial class MinimalController : Controller
	{
		public IActionResult One() => Views.View();
		public IActionResult Two() => Views.View2();
	}
}
""",
			path: @"C:\dev\Project\Features\Xyz/MinimalController.cs");

		var result = TestHelper.RunGenerator(TestHelper.RunArgs.Empty.WithCSharpSource(source1).WithCSharpSource(source2));

		var generatorResult = Assert.Single(result.Results);

		Assert.Equal(3, generatorResult.GeneratedSources.Length);

		_ = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.AttributesFileName));

		var result1 = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace1, ClassName)))
			.SourceText;

		var result2 = Assert.Single(generatorResult.GeneratedSources.Where(
			source => source.HintName == MvcHelperGenerator.GetGeneratedFileNameForClass(Namespace2, ClassName)))
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
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
	}
}

""",
			actual: result1.ToString(),
			ignoreLineEndingDifferences: true);

		Assert.Equal(expected: //lang=c#-test
"""
#nullable enable

using System;

namespace Project.Features.Xyz;

partial class MinimalController
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap(MinimalController controller)
	{
		private readonly MinimalController _controller = controller;
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Views/View.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View2() => 
	_controller.View(viewName: @"/Views/View2.cshtml");
	}
}

""",
			actual: result2.ToString(),
			ignoreLineEndingDifferences: true);
	}



	[Fact]
	public void NamedMapView()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("./View", Name = "Abc")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.Abc();
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
public global::Microsoft.AspNetCore.Mvc.ViewResult Abc() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}


	[Fact]
	public void SameViewDifferentName()
	{
		const string Namespace = "Project.Features.Asd";
		const string ClassName = "MinimalController";

		var source = new CSharpSource(source: //lang=c#-test
"""
using Microsoft.AspNetCore.Mvc;
using MvcHelper;

 // not interpolating the const because it breaks c# highlighting
namespace Project.Features.Asd;

[MapView("./View")]
[MapPartialView("./View", Name = "PartialView")]
public sealed partial class MinimalController : Controller
{
	public IActionResult Index() => Views.View();
	public IActionResult Index2() => Views.PartialView();
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
public global::Microsoft.AspNetCore.Mvc.PartialViewResult PartialView() => 
	_controller.PartialView(viewName: @"/Features/Asd/View.cshtml");
public global::Microsoft.AspNetCore.Mvc.ViewResult View() => 
	_controller.View(viewName: @"/Features/Asd/View.cshtml");
	}
}

""",
		actual: generatedSource.ToString(),
		ignoreLineEndingDifferences: true);
	}
}
