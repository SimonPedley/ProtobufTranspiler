using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Implementations;

namespace ProtobufTranspiler.SourceGenerator;

public class ConstructionContext
{
    private int _generatedVariableIndex;
    
    public GenerationContext Generation;
    public IReadOnlyDictionary<string, ITypeImplementation> TypeImplementations { get; }
    public ServiceDefinition[] ServiceDefinitions { get; }

    public ConstructionContext(GenerationContext generationContext, IReadOnlyDictionary<string,ITypeImplementation> typeImplementations, ServiceDefinition[] serviceDefinitions)
    {
        TypeImplementations = typeImplementations;
        ServiceDefinitions = serviceDefinitions;
        Generation = generationContext;
    }

    public string NewVarName() => $@"genVar{++_generatedVariableIndex:D}";
}
