using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing.Extensions;

namespace ProtobufTranspiler.SourceGenerator.Analysing.Model
{
    /// <summary>
    /// The meta of a type used as a proto message
    /// </summary>
    public class TypeDefinition
    {
        public const string ProtobufIMessageFullTypeName = "Google.Protobuf.IMessage";
        
        public TypeDefinition(INamedTypeSymbol generatedSymbol, bool isExternal)
        {
            GeneratedSymbol = generatedSymbol;
            IsExternal = isExternal;

            Name = GeneratedSymbol.Name;
            Namespace = GeneratedSymbol.ContainingNamespace.ToString();
            NameWithNamespace = GeneratedSymbol.GetNameWithNamespace();
            FullNameWithNamespace = GeneratedSymbol.ToString();
        }

        public string Name { get; }
        public string Namespace { get; }
        public string NameWithNamespace { get; }
        public string FullNameWithNamespace { get; }
        public INamedTypeSymbol GeneratedSymbol { get; }
        public bool IsExternal { get; }
    }
}
