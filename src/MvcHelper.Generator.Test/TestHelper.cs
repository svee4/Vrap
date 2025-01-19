using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace MvcHelper.Generator.Test;

public static class TestHelper
{
	public static GeneratorDriverRunResult RunGenerator(RunArgs arguments)
	{
		var syntaxTrees = arguments.CSharpSources.Select(source =>
			CSharpSyntaxTree.ParseText(source.Source, path: source.Path));

		var compilation = CSharpCompilation.Create(
			assemblyName: "Tests",
			syntaxTrees: syntaxTrees,
			references: Basic.Reference.Assemblies.AspNet90.References.All,
			options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
		);

		var generator = new MvcHelperGenerator();

		var driver = CSharpGeneratorDriver.Create(
			[GeneratorExtensions.AsSourceGenerator(generator)],
			optionsProvider: new TestOptionsProvider());

		driver = (CSharpGeneratorDriver)driver.AddAdditionalTexts(
			arguments.AdditionalTexts.Cast<AdditionalText>().ToImmutableArray());

		driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
				compilation,
				out var outputCompilation,
				out var diagnostics
			);

		var result = driver.GetRunResult();

		var diagnostics2 = outputCompilation
				.GetDiagnostics()
				.Where(d => d.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning);

		Assert.Empty(diagnostics);
		Assert.Empty(diagnostics2);
		return result;
	}

	public readonly struct RunArgs(
		ImmutableArray<CSharpSource> cSharpSources,
		ImmutableArray<CustomAdditionalText> additionalTexts)
	{
		private readonly ImmutableArray<CSharpSource> _cSharpSources = cSharpSources;
		private readonly ImmutableArray<CustomAdditionalText> _additionalTexts = additionalTexts;

		[Obsolete("Use Empty")]
		public RunArgs() : this(default, default) { }

		public readonly ImmutableArray<CSharpSource> CSharpSources =>
			_cSharpSources.IsDefault ? [] : _cSharpSources;

		public readonly ImmutableArray<CustomAdditionalText> AdditionalTexts =>
			_additionalTexts.IsDefault ? [] : _additionalTexts;

		public RunArgs WithCSharpSource(CSharpSource source) =>
			new(CSharpSources.Add(source), AdditionalTexts);

		public RunArgs WithAdditionalText(CustomAdditionalText additionalText) =>
			new(CSharpSources, AdditionalTexts.Add(additionalText));

#pragma warning disable CS0618 // Type or member is obsolete
		public static RunArgs Empty => new();
#pragma warning restore CS0618 // Type or member is obsolete
	}

	internal sealed class TestOptionsProvider : AnalyzerConfigOptionsProvider
	{
		public override AnalyzerConfigOptions GlobalOptions => new TestOptions();
		public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new TestOptions();
		public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new TestOptions();

		internal sealed class TestOptions : AnalyzerConfigOptions
		{
			public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
			{
				switch (key)
				{
					case "build_property.MSBuildProjectDirectory":
						value = @"C:\dev\Project";
						return true;
				}

				value = null;
				return false;
			}
		}
	}
}

public sealed class CSharpSource(string source, string path = "")
{
	public string Source { get; } = source;
	public string Path { get; } = path;
}

public sealed class CustomAdditionalText(string path, string source) : AdditionalText
{
	private readonly SourceText _source = SourceText.From(source);

	public override string Path { get; } = path;

	public override SourceText? GetText(CancellationToken cancellationToken = default) => _source;
}
