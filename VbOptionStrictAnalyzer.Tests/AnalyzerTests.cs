using System.Threading.Tasks;

using Xunit;

using Verifier = Microsoft.CodeAnalysis.VisualBasic.Testing.XUnit.AnalyzerVerifier<VbOptionStrictAnalyzer.OptionStrictShouldNotBeTurnedOff>;

namespace VbOptionStrictAnalyzer.Tests;

public class SampleSyntaxAnalyzerTests
{
    [Fact]
    public async Task ClassWithNoOption_NoDiagnostic()
    {
        const string text = """
                            Public Class Hello
                            End Class
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task ClassWithStrictOn_NoDiagnostic()
    {
        const string text = """
                            Option Strict On
                            
                            Public Class Hello
                            End Class
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task ClassWithDifferentOption_NoDiagnostic()
    {
        const string text = """
                            Option Infer Off

                            Public Class Hello
                            End Class
                            """;

        await Verifier.VerifyAnalyzerAsync(text).ConfigureAwait(false);
    }

    [Fact]
    public async Task ClassWithStrictOff_HasDiagnostic()
    {
        const string text = """
                            Option Strict Off

                            Public Class Hello
                            End Class
                            """;

        var expected = Verifier.Diagnostic("VBOSA0001")
                               .WithSpan(1, 1, 1, 18)
                               .WithArguments("Strict");
        await Verifier.VerifyAnalyzerAsync(text, expected).ConfigureAwait(false);
    }
}