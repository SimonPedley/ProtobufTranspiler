using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ProtobufTranspiler.SourceGenerator;

public class ProtobufSourceGenerator : ISourceGenerator
{
    private readonly IProtobufCodeGenerator _protobufCodeGenerator;

    public ProtobufSourceGenerator()
    {
        _protobufCodeGenerator = new ProtobufCodeGenerator();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var generateSources = _protobufCodeGenerator.AnalyseCompilationAndGenerateSource(new GenerationContext(context.Compilation));
        
        foreach (var generatedSource in generateSources) context.AddSource(generatedSource.FileName, SourceText.From(generatedSource.Code, Encoding.UTF8));
    }
}
