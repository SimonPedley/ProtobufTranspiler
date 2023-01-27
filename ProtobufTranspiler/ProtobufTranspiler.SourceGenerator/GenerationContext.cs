using Microsoft.CodeAnalysis;

namespace ProtobufTranspiler.SourceGenerator;

public class GenerationContext
{
    public Compilation Compilation { get; }

    public GenerationContext(Compilation compilation)
    {
        Compilation = compilation;
    }
}
