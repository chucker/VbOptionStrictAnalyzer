using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

using System.Collections.Immutable;

namespace VbOptionStrictAnalyzer;

/// <summary>
/// Heavily derived from https://johnkoerner.com/csharp/code-analyzers-they-arent-just-for-c/
/// </summary>
[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
public class OptionStrictShouldNotBeTurnedOff : DiagnosticAnalyzer
{
    public const string DiagnosticId = "VBOSA0001";

    internal static readonly LocalizableString Title = "Option strict should not be turned off";
    internal static readonly LocalizableString MessageFormat = "Option {0} should not be turned off";
    internal const string Category = "Visual Basic";

    internal static DiagnosticDescriptor Rule =
        new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.OptionStatement);
    }

    private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not OptionStatementSyntax optionStatement)
            return;

        if (optionStatement.NameKeyword.IsMissing || optionStatement.ValueKeyword.IsMissing)
            return;

        if (!optionStatement.NameKeyword.IsKind(SyntaxKind.StrictKeyword))
            return;

        if (!optionStatement.ValueKeyword.IsKind(SyntaxKind.OffKeyword))
            return;

        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), optionStatement.NameKeyword.ValueText);

        context.ReportDiagnostic(diagnostic);
    }
}