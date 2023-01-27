using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;
using ProtobufTranspiler.SourceGenerator.TypeOverriding;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class OverriddenImplementation : ITypeImplementation
    {
        protected readonly ITypeOverride TypeOverride;

        protected readonly string Namespace;
        
        public TypeDefinition Protobuf { get; }

        public virtual IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context)
        {
            var code = new StringBuilder();

            code.DeclareNamespace(Namespace, () =>
            {
                code.DeclareClass(
                    TypeOverride.ConverterName,
                    isPublic: false,
                    isStatic: true,
                    content: () =>
                    {
                        code.DeclareMethod(
                            "Transform",
                            modifiers: "static",
                            parameters: new[]
                            {
                                $@"{TypeOverride.ProtoNameWithNamespace} source"
                            },
                            returns: TypeOverride.FlowwNameWithNamespace,
                            content: () => TypeOverride.BuildProtoToTargetAssignment(code));
                        
                        code.DeclareMethod(
                            "Transform",
                            modifiers: "static",
                            parameters: new[]
                            {
                                $@"{TypeOverride.FlowwNameWithNamespace} source"
                            },
                            returns: TypeOverride.ProtoNameWithNamespace,
                            content: () => TypeOverride.BuildTargetToProtoAssignment(code));
                    });
            });
            
            yield return new GeneratedSource($@"{Namespace}.{TypeOverride.ConverterName}.override", code.ToString());
        }

        public virtual string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false) => TypeOverride.FlowwNameWithNamespace;
        
        public string GetDefaultValue(bool explicitlyNullable = false) => "default";

        public virtual void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto)
        {
            code.AppendLine($@"{destination} = {TypeOverride.ConverterName}.Transform({source});");
        }

        protected OverriddenImplementation(TypeDefinition protoTypeDefinition, ITypeOverride typeOverride)
        {
            TypeOverride = typeOverride;
            Namespace = $@"{protoTypeDefinition.Namespace}.Generated";
            Protobuf = protoTypeDefinition;
        }

        public static ITypeImplementation? TryImplement(TypeDefinition typeDefinition)
        {
            throw new NotImplementedException();
            // var matchingTypeOverride = Config.TypeOverrides.FirstOrDefault(typeOverride => typeOverride.ProtoNameWithNamespace == typeDefinition.FullNameWithNamespace);
            // return matchingTypeOverride != null ? new OverriddenImplementation(typeDefinition, matchingTypeOverride) : null;
        }
    }
}
