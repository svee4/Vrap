using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace MvcHelper.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed partial class MvcHelperAnalyzer : DiagnosticAnalyzer
{

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Diagnostics.DoesNotInheritFromMvcController];

	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

		context.RegisterSymbolAction(EnsureClassDerivesFromController, SymbolKind.NamedType);
	}

	private static void EnsureClassDerivesFromController(SymbolAnalysisContext context)
	{
		if (!context.Symbol.GetAttributes().Any(attr => attr.AttributeClass?.MetadataName
			is MvcHelperGenerator.MapViewNonGenericAttributeName or MvcHelperGenerator.MapViewGenericAttributeName))
		{
			return;
		}

		if (context.Symbol is not INamedTypeSymbol symbol)
		{
			return;
		}

		// must inherit from because the sg relies on a method from that base class Microsoft.AspNetCore.Mvc.Controller
		var baseType = symbol.BaseType;
		while (baseType is not null)
		{
			if (baseType.MetadataName is "Microsoft.AspNetCore.Mvc.Controller")
			{
				break;
			}
			baseType = baseType.BaseType;
		}

		if (baseType is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(
				Diagnostics.DoesNotInheritFromMvcController,
				symbol.Locations.First(),
				symbol.Name
			));
		}
	}
}
