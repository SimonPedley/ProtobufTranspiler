using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class AsIsImplementation : ITypeImplementation
    {
        public AsIsImplementation(TypeDefinition protoTypeDefinition)
        {
            Protobuf = protoTypeDefinition;
        }

        public TypeDefinition Protobuf { get; }
        
        public virtual IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context) => Enumerable.Empty<GeneratedSource>();

        public virtual string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false) => !Protobuf.GeneratedSymbol.IsReferenceType && explicitlyNullable ? $@"{Protobuf.FullNameWithNamespace}?" : Protobuf.FullNameWithNamespace;

        public string GetDefaultValue(bool explicitlyNullable = false) => "default";

        public virtual void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto)
        {
            if (!explicitlyNullable)
            {
                code.AppendLine($@"{destination} = {source};");
                return;
            }
            
            code.AsStatement(() =>
            {
                var pathObjectDivider = source.LastIndexOf('.');
                var sourcePath = pathObjectDivider >= 0 ? source.Substring(0, pathObjectDivider) : null;
                var sourceObject = source.Substring(pathObjectDivider + 1);
            
                if (toProto || sourcePath is null)
                {
                    code.AppendLine(Protobuf.GeneratedSymbol.IsReferenceType
                        ? $@"if ({source} != null) {destination} = {source};"
                        : $@"if ({source}.HasValue) {destination} = {source}.Value;");
                    return;
                }

                code.Append($@"{destination} = {sourcePath}.Has{sourceObject} ? ");
                code.Append(Protobuf.GeneratedSymbol.IsReferenceType ? source : $@"({GetFlowwFullTypeNameWithNamespace(true)}){source}");
                code.Append(@" : null");
            });
        }

        public static ITypeImplementation Implement(TypeDefinition typeDefinition) => new AsIsImplementation(typeDefinition);
    }
}
