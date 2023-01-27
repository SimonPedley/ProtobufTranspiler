using System.Text;
using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class EnumImplementation : ITypeImplementation
    {
        public const string CastByStringStaticMethodNameWithNamespace = @"Floww.Libraries.Grpc.Extensions.EnumExtensions.CastByString";

        protected readonly string Name;
        protected readonly string Namespace;
        protected readonly string FullNameWithNamespace;
        protected readonly Dictionary<string, int> EnumValues;

        public TypeDefinition Protobuf { get; }

        public virtual IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context)
        {
            if (Protobuf.IsExternal) yield break;

            var code = new StringBuilder();

            code.DeclareNamespace(Namespace, () => { code.DeclareEnum(GetFlowwFullTypeNameWithNamespace(), EnumValues); });

            yield return new GeneratedSource($@"{Protobuf.Namespace}.{GetFlowwFullTypeNameWithNamespace()}", code.ToString());
        }

        public virtual string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false)
        {
            var propertyTypeNameWithNamespace = Protobuf.IsExternal ? Protobuf.FullNameWithNamespace : FullNameWithNamespace;
            if (explicitlyNullable) propertyTypeNameWithNamespace = $@"{propertyTypeNameWithNamespace}?";

            return propertyTypeNameWithNamespace;
        }

        public string GetDefaultValue(bool explicitlyNullable = false) => "default";

        public virtual void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto)
        {
            var flowwTypeFullNameWithNamespace = GetFlowwFullTypeNameWithNamespace();
            
            code.AsStatement(() =>
            {
                if (toProto)
                {
                    if (explicitlyNullable)
                    {
                        code.Append($@"if ({source}.HasValue) ");
                        source = $@"{source}.Value";
                    }
                    
                    code.Append($@"{destination} = ");
                    code.Append(Protobuf.IsExternal
                        ? source
                        : $@"{CastByStringStaticMethodNameWithNamespace}<{flowwTypeFullNameWithNamespace}, {Protobuf.FullNameWithNamespace}>({source})");
                    
                    return;
                }

                code.Append($@"{destination} = ");
                
                if (explicitlyNullable)
                {
                    var pathObjectDivider = source.LastIndexOf('.');
                    var sourcePath = pathObjectDivider >= 0 ? source.Substring(0, pathObjectDivider) : null;
                    var sourceObject = source.Substring(pathObjectDivider + 1);
            
                    code.Append($@"!{sourcePath}.Has{sourceObject} ? ({flowwTypeFullNameWithNamespace}?)null : ");
                }
                
                code.Append(Protobuf.IsExternal
                    ? source
                    : $@"{CastByStringStaticMethodNameWithNamespace}<{Protobuf.FullNameWithNamespace}, {flowwTypeFullNameWithNamespace}>({source})");
            });
        }

        protected EnumImplementation(TypeDefinition typeDefinition)
        {
            Protobuf = typeDefinition;
            Name = Protobuf.Name;
            Namespace = $@"{Protobuf.Namespace}.Generated";
            FullNameWithNamespace = $@"{Namespace}.{Name}";
            EnumValues = Protobuf.GeneratedSymbol
                .GetMembers()
                .OfType<IFieldSymbol>()
                .ToDictionary(
                    fieldSymbol => fieldSymbol.Name,
                    fieldSymbol => (int)(fieldSymbol.ConstantValue ?? 0));
        }

        public static ITypeImplementation? TryImplement(TypeDefinition typeDefinition)
        {
            var isEnum = typeDefinition.GeneratedSymbol.TypeKind == TypeKind.Enum;

            return isEnum ? new EnumImplementation(typeDefinition) : null;
        }
    }
}
