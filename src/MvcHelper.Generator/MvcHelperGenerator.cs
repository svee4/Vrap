using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MvcHelper.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class MvcHelperGenerator : IIncrementalGenerator
{
	public const string BaseFileName = "MvcHelper.Generator";
	public const string AttributesFileName = $"{BaseFileName}.Attributes.g.cs";
	public const string MapViewNonGenericAttributeName = "MvcHelper.MapViewAttribute";
	public const string MapViewGenericAttributeName = "MvcHelper.MapViewAttribute`1";
	public const string MapPartialViewNonGenericAttributeName = "MvcHelper.MapPartialViewAttribute";
	public const string MapPartialViewGenericAttributeName = "MvcHelper.MapPartialViewAttribute`1";

	public static string GetGeneratedFileNameForClass(string classNamespace, string className)
	{
		var name = $"{classNamespace}.{className}".Replace('.', '_').Replace('`', '_');
		return $"{BaseFileName}.Classes.{name}.g.cs";
	}

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		RegisterAttributesOutput(context);

		// this generator lets you utilize relative paths,
		// as in, relative to the .cs file.
		// we need to transform those paths to be relative to the project root
		// for the mvc view engine to use.
		// im not actually sure what exactly the engine considers to be its root
		// but project file directory works fine so far.
		var projectFileDirectoryProvider = context.AnalyzerConfigOptionsProvider.Select(
			static (options, token) =>
		{
			_ = options.GlobalOptions.TryGetValue("build_property.MSBuildProjectDirectory", out var value);
			return value;
		});

		var normalNonGenerics = context.SyntaxProvider.ForAttributeWithMetadataName(
				MapViewNonGenericAttributeName,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: static (context, token) => TransformNonGenericWithViewType(ViewType.Normal, context, token))
			.WhereNotNull();

		var normalGenerics = context.SyntaxProvider.ForAttributeWithMetadataName(
				MapViewGenericAttributeName,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: static (context, token) => TransformGenericWithViewType(ViewType.Normal, context, token))
			.WhereNotNull();

		var partialNonGenerics = context.SyntaxProvider.ForAttributeWithMetadataName(
			MapPartialViewNonGenericAttributeName,
			predicate: static (node, _) => node is ClassDeclarationSyntax,
			transform: static (context, token) => TransformNonGenericWithViewType(ViewType.Partial, context, token))
		.WhereNotNull();

		var partialGenerics = context.SyntaxProvider.ForAttributeWithMetadataName(
			MapPartialViewGenericAttributeName,
			predicate: static (node, _) => node is ClassDeclarationSyntax,
			transform: static (context, token) => TransformGenericWithViewType(ViewType.Partial, context, token))
		.WhereNotNull();

		// combines data from all attributes on the same class into one
		// this is somewhat incremental but probably not very performant
		// but it shouldnt matter that much because the earlier incrementality only breaks
		// when the attributes are modified
		var combined = normalNonGenerics.Collect()
			.Combine(normalGenerics.Collect())
			.Combine(partialNonGenerics.Collect())
			.Combine(partialGenerics.Collect())
			.Combine(projectFileDirectoryProvider)
			.SelectMany(static (item, token) =>
		{
			var (left1, projectFileDirectory) = item;
			var (left2, partialGenerics) = left1;
			var (left3, partialNonGenerics) = left2;
			var (normalNonGenerics, normalGenerics) = left3;

			if (projectFileDirectory is null)
			{
				return [];
			}

			var classViewDatasMap = (Dictionary<ClassData, List<ViewData>>)[];
			var datas = partialGenerics.Concat(partialNonGenerics).Concat(normalGenerics).Concat(normalNonGenerics);

			foreach (var data in datas)
			{
				token.ThrowIfCancellationRequested();

				var key = data.ClassData;
				var values = data.ViewDatas;

				if (!classViewDatasMap.TryGetValue(key, out var viewDatas))
				{
					viewDatas = [];
					classViewDatasMap.Add(key, viewDatas);
				}

				viewDatas.AddRange(values);
			}

			return classViewDatasMap.Select(kvp => new SourceActionData(
				new Data(kvp.Key, kvp.Value.CopyToEquatableReadOnlyList()),
				projectFileDirectory));
		});

		context.RegisterSourceOutput(combined, SourceAction);
	}


	internal static Data? TransformNonGenericWithViewType(
		ViewType type,
		GeneratorAttributeSyntaxContext context,
		CancellationToken token)
	{
		var attributes = context.Attributes;
		var viewDatas = attributes.Length > 8
			? (Span<ViewData>)new ViewData[attributes.Length]
			: [default, default, default, default, default, default, default, default];

		viewDatas = viewDatas[..attributes.Length];

		for (var i = 0; i < attributes.Length; i++)
		{
			token.ThrowIfCancellationRequested();

			var attr = context.Attributes[i];
			if (attr.ConstructorArguments.SafeSingleOrDefault().Value?.ToString() is not { } viewPath)
			{
				return null;
			}

			viewDatas[i] = new ViewData(viewPath, type, null, GetName(attr));
		}

		return new Data(ExtractClassData(context), viewDatas.CopyToEquatableReadOnlyList());
	}

	internal static Data? TransformGenericWithViewType(
		ViewType type,
		GeneratorAttributeSyntaxContext context,
		CancellationToken token)
	{
		var attributes = context.Attributes;
		var viewDatas = attributes.Length > 8
			? (Span<ViewData>)new ViewData[attributes.Length]
			: [default, default, default, default, default, default, default, default];

		viewDatas = viewDatas[..attributes.Length];

		for (var i = 0; i < attributes.Length; i++)
		{
			token.ThrowIfCancellationRequested();

			var attr = attributes[i];
			if (attr.ConstructorArguments.SafeSingleOrDefault().Value?.ToString() is not { } viewPath)
			{
				return null;
			}

			if (attr
				.AttributeClass?
				.TypeArguments
				.SafeSingleOrDefault()?
				.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				is not { } modelType)
			{
				return null;
			}

			viewDatas[i] = new ViewData(viewPath, type, modelType, GetName(attr));
		}

		return new Data(ExtractClassData(context), viewDatas.CopyToEquatableReadOnlyList());
	}

	private static string? GetName(AttributeData attr)
	{
		if (attr.NamedArguments.SafeSingleOrDefault() is { } kvp
			&& kvp.Key == "Name"
			&& !kvp.Value.IsNull
			&& kvp.Value.Type?.SpecialType == SpecialType.System_String
			&& kvp.Value.Value is string name)
		{
			return name;
		}
		else
		{
			return null;
		}
	}

	internal static ClassData ExtractClassData(GeneratorAttributeSyntaxContext context)
	{
		var filepath = context.TargetNode.SyntaxTree.FilePath;
		return new ClassData(
			context.TargetSymbol.ContainingNamespace.ToDisplayString(),
			context.TargetSymbol.Name,
			filepath);
	}

	internal static IEnumerable<ParsedViewData> ParseViewDatas(SourceActionData sourceData) =>
		sourceData.Data.ViewDatas
			.Select(viewData =>
			{
				var classFilePath = NormalizePath(sourceData.Data.ClassFilePath);
				var viewFilePath = NormalizePath(viewData.FilePath);
				var projectRootPath = NormalizePath(sourceData.ProjectFileDirectory);

				viewFilePath = ResolveProjectRelativePath(viewFilePath, classFilePath, sourceData.ProjectFileDirectory);
				viewFilePath = AddFileExtension(viewFilePath);

				var methodName = viewData.MethodName ?? GetViewName(viewFilePath);
				return new ParsedViewData(methodName, viewFilePath, viewData.ModelType, viewData.Type);

				static string NormalizePath(string path) => path.Replace("\\", "/");

				static string ResolveProjectRelativePath(string viewPath, string classPath, string projectRootPath)
				{
					string path;
					if (viewPath[0] is '/')
					{
						path = FuckingPathThing(projectRootPath, viewPath);
					}
					else
					{
						// this would make a valid path but i strip it for easier testing
						if (viewPath.StartsWith("./", StringComparison.InvariantCulture))
						{
							viewPath = viewPath[2..];
						}

						var directory = Path.GetDirectoryName(classPath);
						var absolutePath = Path.Combine(directory, viewPath);
						path = FuckingPathThing(projectRootPath, absolutePath);
					}

					path = NormalizePath(path);
					return path;

					static string FuckingPathThing(string relativeTo, string path)
					{
						return path.Replace(relativeTo, "");
					}
				}

				static string AddFileExtension(string path)
				{
					if (!Path.HasExtension(path))
					{
						path = Path.ChangeExtension(path, ".cshtml");
					}

					return NormalizePath(path);
				}

				static string GetViewName(string viewPath) =>
					Path.GetFileNameWithoutExtension(viewPath);
			});

	internal sealed record ParsedViewData(string MethodName, string Path, string? Model, ViewType Type);

	internal static void SourceAction(SourceProductionContext context, SourceActionData sourceData)
	{
		var data = sourceData.Data;

		// so sorry for this orderby but it needs to be deterministic for tests
		var methods = ParseViewDatas(sourceData)
			.OrderBy(data => data.MethodName)
			.Select(view =>
			{
				var method = view.Type switch
				{
					ViewType.Normal => "View",
					ViewType.Partial => "PartialView",
					_ => throw new InvalidOperationException($"Unknown ViewType {view.Type}"),
				};

				return $$"""
public global::Microsoft.AspNetCore.Mvc.{{method}}Result {{view.MethodName}}({{IfNotNull(view.Model, $"{view.Model} model")}}) => 
	_controller.{{method}}(viewName: @"{{view.Path}}"{{IfNotNull(view.Model, $", model: model")}});
""";
			});

		var source =
$$"""
#nullable enable

using System;

namespace {{data.Namespace}};

partial class {{data.ClassName}}
{
	private ViewMap? _views;
	private ViewMap Views => _views ??= new ViewMap(this);

	private sealed class ViewMap({{data.ClassName}} controller)
	{
		private readonly {{data.ClassName}} _controller = controller;
{{string.Join("\n", methods)}}
	}
}

""";

		context.AddSource(
			GetGeneratedFileNameForClass(data.Namespace, data.ClassName),
			SourceText.From(source, Encoding.UTF8));
	}

	internal static void RegisterAttributesOutput(IncrementalGeneratorInitializationContext context) =>
		context.RegisterPostInitializationOutput(static context2 =>
		{
			var attributes = (List<string>)[
				"MapViewAttribute",
				"MapViewAttribute<TModel>",
				"MapPartialViewAttribute",
				"MapPartialViewAttribute<TModel>"
			];

			var strings = attributes.Select(attr =>
$$"""
[global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
public sealed class {{attr}}(string viewPath) : global::System.Attribute
{
	public string ViewPath { get; } = viewPath;
	public string Name { get; init; }
}
""");

			context2.AddSource(
				AttributesFileName,
				SourceText.From(
					encoding: Encoding.UTF8,
					text: //lang=c#-test
				$$"""
namespace MvcHelper;

{{string.Join("\n\n", strings)}}
"""
			));
		});

	internal sealed record SourceActionData(Data Data, string ProjectFileDirectory);

	internal sealed record Data(ClassData ClassData, EquatableReadOnlyList<ViewData> ViewDatas)
	{
		public string Namespace => ClassData.Namespace;
		public string ClassName => ClassData.ClassName;
		public string ClassFilePath => ClassData.ClassFilePath;
	}

	internal sealed record ClassData(string Namespace, string ClassName, string ClassFilePath);

	internal readonly record struct ViewData(string FilePath, ViewType Type, string? ModelType, string? MethodName);

	internal enum ViewType
	{
		Normal = 1,
		Partial
	}

	internal static string? IfNotNull(string? source, string then) => source is null ? null : then;
}

internal static class Extensions
{
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
	{
		foreach (var value in source)
		{
			if (value is not null)
			{
				yield return value;
			}
		}
	}

	public static T? SafeSingleOrDefault<T>(this IEnumerable<T> source)
	{
		using var enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return default;
		}

		var value = enumerator.Current;
		if (enumerator.MoveNext())
		{
			return default;
		}

		return value;
	}

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
	public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> source) where T : class =>
		source.Where(data => data is not null);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
}
