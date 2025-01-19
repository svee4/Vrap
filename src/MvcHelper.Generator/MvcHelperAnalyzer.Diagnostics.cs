using Microsoft.CodeAnalysis;

namespace MvcHelper.Generator;

partial class MvcHelperAnalyzer
{
	public static class Diagnostics
	{
		public static DiagnosticDescriptor DoesNotInheritFromMvcController { get; } = new DiagnosticDescriptor(
			id: "MVCH001",
			title: "Type with ViewMap must inherit from Microsoft.AspNetCore.Mvc.Controller",
			messageFormat: "Type {0} must inherit from Microsoft.AspNetCore.Mvc.Controller because it contains a ViewMap",
			category: "MvcHelper",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true
		);
	}
}
