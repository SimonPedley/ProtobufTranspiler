using System.Text;
using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing.Extensions;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class PaginationImplementation : MessageImplementation
    {
        public const string ProtobufPaginationInfoTypeNameWithNamespace = "Proto.Floww.PaginationInfo";
        public const string FlowwPagedResponseDtoTypeNameWithNamespace = "Floww.Libraries.Common.PagedResponseDto";

        protected readonly TypeDefinition ResultsTypeDefinition;
        
        protected PaginationImplementation(TypeDefinition protoTypeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitionLookup,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup) : base(protoTypeDefinition, typeDefinitionLookup, typeImplementationLookup)
        {
            var resultsTypeFullNameAndNamespace = PropertyDefinitions.First(definition => definition.Name == "Results").Definition.GeneratedSymbol.TypeArguments.First().ToString();
            ResultsTypeDefinition = typeDefinitionLookup[resultsTypeFullNameAndNamespace];
        }

        public override IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context)
        {
            if (Protobuf.IsExternal) yield break;

            var code = new StringBuilder();

            code.DeclareNamespace(Namespace, () =>
            {
                var baseType = TypeImplementationLookup[ResultsTypeDefinition.FullNameWithNamespace].GetFlowwFullTypeNameWithNamespace();

                code.DeclareClass(
                    Name,
                    inherits: $@"{FlowwPagedResponseDtoTypeNameWithNamespace}<{baseType}>",
                    content: () =>
                    {
                        foreach (var propertyDefinition in PropertyDefinitions)
                        {
                            if (propertyDefinition.Name == "Results") continue;

                            var typeImplementation = TypeImplementationLookup[propertyDefinition.Definition.FullNameWithNamespace];
                            code.DeclareProperty(
                                propertyDefinition.Name,
                                typeImplementation.GetFlowwFullTypeNameWithNamespace(propertyDefinition.ExplicitlyNullable),
                                typeImplementation.GetDefaultValue(propertyDefinition.ExplicitlyNullable));
                        }

                        code.AppendLine($@"public {Name}() {{ }}");

                        code.Append($@"public {Name}({FlowwPagedResponseDtoTypeNameWithNamespace}<{baseType}> source)");
                        code.WrapBracesAround(() =>
                        {
                            code.AppendLine("Count = source.Count;");
                            code.AppendLine("Results = source.Results;");
                            code.AppendLine("CurrentPage = source.CurrentPage;");
                            code.AppendLine("PageCount = source.PageCount;");
                            code.AppendLine("PageSize = source.PageSize;");
                        });
                    });
            });

            yield return new GeneratedSource($@"{GetFlowwFullTypeNameWithNamespace()}.message", code.ToString());

            code = new StringBuilder();

            code.DeclareNamespace(Namespace, () =>
            {
                code.DeclareClass(
                    ConverterName,
                    isPublic: false,
                    isStatic: true,
                    content: () =>
                    {
                        code.DeclareMethod(
                            "Transform",
                            modifiers: "static",
                            returns: Protobuf.FullNameWithNamespace,
                            parameters: new[] { $@"{GetFlowwFullTypeNameWithNamespace()} source" },
                            content: () =>
                            {
                                code.DeclareIfStatement(
                                    () => code.Append(@"source == null"),
                                    () => code.Append(@"return null;"));

                                var destination = code.DeclareVariable(context, assignment: () => code.Append($@"new {Protobuf.FullNameWithNamespace}()"));
                                var paginationInfoTypeImplementation = TypeImplementationLookup[ProtobufPaginationInfoTypeNameWithNamespace];
                                
                                code.AppendLine($@"{destination}.Base = new {ProtobufPaginationInfoTypeNameWithNamespace}();");
                                WriteMessageProperties(code, context, @"source", $@"{destination}.Base", paginationInfoTypeImplementation, true);
                                WriteMessageProperties(code, context, @"source", destination, this, true);
                                
                                code.AppendLine($@"return {destination};");
                            });
                        
                        code.DeclareMethod(
                            "Transform",
                            modifiers: "static",
                            returns: GetFlowwFullTypeNameWithNamespace(),
                            parameters: new[] { $@"{Protobuf.FullNameWithNamespace} source" },
                            content: () =>
                            {
                                code.DeclareIfStatement(
                                    () => code.Append(@"source == null"),
                                    () => code.Append(@"return null;"));
                                
                                var destination = code.DeclareVariable(context, assignment: () => code.Append($@"new {GetFlowwFullTypeNameWithNamespace()}()"));
                                var paginationInfoTypeImplementation = TypeImplementationLookup[ProtobufPaginationInfoTypeNameWithNamespace];
                                
                                WriteMessageProperties(code, context, $@"source.Base", destination, paginationInfoTypeImplementation, false);
                                WriteMessageProperties(code, context, @"source", destination, this, false);
                                
                                code.AppendLine($@"return {destination};");
                            });
                    });
            });

            yield return new GeneratedSource($@"{ConverterNameWithNamespace}.converter", code.ToString());
        }

        public new static ITypeImplementation? TryImplement(TypeDefinition typeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitions,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            var propertySymbols = typeDefinition.GeneratedSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .ToArray();

            var isPaginationWrapper = propertySymbols.Any(symbol => symbol.Name == "Results" && symbol.Type.GetNameWithNamespace() == CollectionImplementation.ProtobufRepeatedFieldTypeNameWithNamespace) &&
                                      propertySymbols.Any(symbol => symbol.Name == "Base" && symbol.Type.GetNameWithNamespace() == ProtobufPaginationInfoTypeNameWithNamespace);

            return isPaginationWrapper ? new PaginationImplementation(typeDefinition, typeDefinitions, typeImplementationLookup) : null;
        }
    }
}
