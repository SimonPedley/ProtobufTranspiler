using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public interface ITypeImplementation
    {
        TypeDefinition Protobuf { get; }
        IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context);
        string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false);
        string GetDefaultValue(bool explicitlyNullable = false);
        void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto);
    }
}
