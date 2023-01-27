using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class CollectionImplementation : ITypeImplementation
    {
        public const string ProtobufRepeatedFieldTypeNameWithNamespace = @"Google.Protobuf.Collections.RepeatedField";
        public const string FlowwEnumerationTypeNameWithNamespace = @"System.Collections.Generic.List";

        protected readonly IReadOnlyDictionary<string, ITypeImplementation> TypeImplementationLookup;
        protected readonly TypeDefinition CollectionType;

        public TypeDefinition Protobuf { get; }

        public virtual IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context) => Enumerable.Empty<GeneratedSource>();

        public virtual string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false)
        {
            var collectionTypeNameWithNamespace = TypeImplementationLookup[CollectionType.FullNameWithNamespace].GetFlowwFullTypeNameWithNamespace(explicitlyNullable);
            return $@"{FlowwEnumerationTypeNameWithNamespace}<{collectionTypeNameWithNamespace}>";
        }

        public string GetDefaultValue(bool explicitlyNullable = false)
        {
            var collectionTypeNameWithNamespace = TypeImplementationLookup[CollectionType.FullNameWithNamespace].GetFlowwFullTypeNameWithNamespace(explicitlyNullable);
            return $@"new {FlowwEnumerationTypeNameWithNamespace}<{collectionTypeNameWithNamespace}>()";
        }

        public virtual void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto)
        {
            var collectionTypeImplementation = TypeImplementationLookup[CollectionType.FullNameWithNamespace];
            var newUpType = toProto
                ? collectionTypeImplementation.Protobuf.FullNameWithNamespace
                : collectionTypeImplementation.GetFlowwFullTypeNameWithNamespace(explicitlyNullable);

            var transformedSourceVariableName = code.DeclareVariable(context, assignment: () =>
            {
                code.Append($@"{source} == null ? System.Linq.Enumerable.Empty<{newUpType}>() : System.Linq.Enumerable.Select");
                code.WrapParenthesisAround(() =>
                {
                    code.Append($@"{source}, ");
                    code.Append(@"source => ");
                    code.WrapBracesAround(() =>
                    {
                        var transformationVariableName = code.DeclareVariable(context, newUpType);
                        collectionTypeImplementation.WriteAssignment(code, context, "source", transformationVariableName, false, toProto);
                        code.AppendLine($@"return {transformationVariableName};");
                    });
                });
            });

            var listVariableName = code.DeclareVariable(context, assignment: () => code.Append($"System.Linq.Enumerable.ToList({transformedSourceVariableName})"));

            code.AsStatement(() =>
            {
                code.Append(destination);
                code.Append(toProto ? $@".Add({listVariableName})" : $@" = {listVariableName}");
            });
        }

        protected CollectionImplementation(TypeDefinition protoTypeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitionLookup,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            Protobuf = protoTypeDefinition;
            TypeImplementationLookup = typeImplementationLookup;

            CollectionType = typeDefinitionLookup[Protobuf.GeneratedSymbol.TypeArguments.First().ToString()];
        }

        public static ITypeImplementation? TryImplement(TypeDefinition typeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitionLookup,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            var isCollection = typeDefinition.NameWithNamespace == ProtobufRepeatedFieldTypeNameWithNamespace;
            return isCollection ? new CollectionImplementation(typeDefinition, typeDefinitionLookup, typeImplementationLookup) : null;
        }
    }
}
