using ProtobufTranspiler.SourceGenerator.Analysing;
using ProtobufTranspiler.SourceGenerator.Building;

namespace ProtobufTranspiler.SourceGenerator;

public interface IProtobufCodeGenerator
{
    IEnumerable<GeneratedSource> AnalyseCompilationAndGenerateSource(GenerationContext context);
}

public class ProtobufCodeGenerator : IProtobufCodeGenerator
{
    public IEnumerable<GeneratedSource> AnalyseCompilationAndGenerateSource(GenerationContext context)
    {
        var constructionContext = AnalyseCompilationAndPlanConstruction(context);
        if (constructionContext.ServiceDefinitions.Length <= 0) return Enumerable.Empty<GeneratedSource>();

        return Enumerable.Empty<GeneratedSource>()
            .Concat(TypeBuilder.Build(constructionContext))
            .Concat(ServiceBuilder.Build(constructionContext))
            .Append(RegistryBuilder.Build(constructionContext));
    }

    private static ConstructionContext AnalyseCompilationAndPlanConstruction(GenerationContext context)
    {
        var typeImplementationLookup = ProtobufMessageAnalyser.Analyse(context);
        var serviceDefinitions = ProtobufServiceAnalyser.Analyse(context, typeImplementationLookup).ToArray();

        return new ConstructionContext(context, typeImplementationLookup, serviceDefinitions);
    }
}
