using System.Text;
using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing;
using ProtobufTranspiler.SourceGenerator.Analysing.Extensions;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Implementations
{
    public class MessageImplementation : ITypeImplementation
    {
        public const string ProtobufIMessageTypeNameWithNamespace = "Google.Protobuf.IMessage";
        public const string ProtobufEntityCreatedResponseTypeNameWithNamespace = "Proto.Floww.EntityCreatedResponse";
        public const string ProtobufEntityUpdatedResponseTypeNameWithNamespace = "Proto.Floww.EntityUpdatedResponse";

        protected readonly IReadOnlyDictionary<string, ITypeImplementation> TypeImplementationLookup;
        protected readonly string Name;
        protected readonly string Namespace;
        protected readonly string NameWithNamespace;
        protected readonly TypeDefinition? Base;
        protected readonly IReadOnlyCollection<PropertyDefinition> PropertyDefinitions;
        protected readonly string ConverterName;
        protected readonly string ConverterNameWithNamespace;

        public TypeDefinition Protobuf { get; }


        public virtual IEnumerable<GeneratedSource> GenerateTypes(ConstructionContext context)
        {
            switch (Protobuf.NameWithNamespace)
            {
                case ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace:
                    yield break;
                case ProtobufEntityCreatedResponseTypeNameWithNamespace:
                case ProtobufEntityUpdatedResponseTypeNameWithNamespace:
                    break;
                default:
                    if (Protobuf.IsExternal) yield break;
                    break;
            }
            
            var code = new StringBuilder();

            code.DeclareNamespace(Namespace, () =>
            {
                code.DeclareClass(
                    Name,
                    inherits: Base != null ? TypeImplementationLookup[Base.FullNameWithNamespace].GetFlowwFullTypeNameWithNamespace() : null,
                    content: () =>
                    {
                        foreach (var propertyDefinition in PropertyDefinitions)
                        {
                            var typeImplementation = TypeImplementationLookup[propertyDefinition.Definition.FullNameWithNamespace];
                            code.DeclareProperty(
                                propertyDefinition.Name,
                                typeImplementation.GetFlowwFullTypeNameWithNamespace(propertyDefinition.ExplicitlyNullable),
                                typeImplementation.GetDefaultValue(propertyDefinition.ExplicitlyNullable));
                        }
                        
                        code.AppendLine($@"public {Name}() {{ }}");
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
                                
                                if (Base != null)
                                {
                                    var baseTypeImplementation = TypeImplementationLookup[Base.FullNameWithNamespace];

                                    baseTypeImplementation.WriteAssignment(code, context, @"source", $@"{destination}.Base", false, true);
                                }

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

                                if (Base != null)
                                {
                                    var baseTypeImplementation = TypeImplementationLookup[Base.FullNameWithNamespace];

                                    WriteMessageProperties(code, context, @"source.Base", destination, baseTypeImplementation, false);
                                }

                                WriteMessageProperties(code, context, @"source", destination, this, false);

                                code.AppendLine($@"return {destination};");
                            });
                    });
            });

            yield return new GeneratedSource($"{ConverterNameWithNamespace}.converter", code.ToString());
        }

        public virtual string GetFlowwFullTypeNameWithNamespace(bool explicitlyNullable = false) => NameWithNamespace;

        public string GetDefaultValue(bool explicitlyNullable = false) => "default";


        public virtual void WriteAssignment(StringBuilder code, ConstructionContext context, string source, string destination, bool explicitlyNullable, bool toProto)
        {
            code.AppendLine($@"{destination} = {ConverterNameWithNamespace}.Transform({source});");
        }

        protected MessageImplementation(TypeDefinition protoTypeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitionLookup,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            Protobuf = protoTypeDefinition;
            Name = Protobuf.Name;
            Namespace = $"{Protobuf.Namespace}.Generated";
            NameWithNamespace = $@"{Namespace}.{Name}";
            TypeImplementationLookup = typeImplementationLookup;
            ConverterName = $@"{Name}Converter";
            ConverterNameWithNamespace = $@"{Namespace}.{ConverterName}";

            var propertySymbols = Protobuf.GeneratedSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .ToArray();

            var basePropertySymbol = propertySymbols.FirstOrDefault(propertySymbol => propertySymbol.Name == "Base");
            if (basePropertySymbol != null)
            {
                Base = typeDefinitionLookup[basePropertySymbol.Type.ToString()];

                propertySymbols = propertySymbols
                    .Where(propertySymbol => propertySymbol.Name != "Base")
                    .ToArray();
            }

            var explicitlyNullablePropertyNames = propertySymbols
                .Where(propertySymbol => propertySymbol.Name.StartsWith("Has") && propertySymbol.IsReadOnly && propertySymbol.Type.ToString() == "bool")
                .Select(propertySymbol => propertySymbol.Name.Substring(3))
                .ToArray();

            PropertyDefinitions = propertySymbols
                .Where(symbol => !symbol.IsReadOnly || symbol.Type.GetNameWithNamespace() == CollectionImplementation.ProtobufRepeatedFieldTypeNameWithNamespace)
                .Select(
                    propertySymbol =>
                    {
                        var propertyFullTypeNameWithNamespace = propertySymbol.Type.ToString();
                        var matchingTypeDefinition = typeDefinitionLookup[propertyFullTypeNameWithNamespace];
                        var isExplicitlyNullable = explicitlyNullablePropertyNames.Contains(propertySymbol.Name);

                        return new PropertyDefinition(propertySymbol.Name, matchingTypeDefinition, isExplicitlyNullable);
                    })
                .ToArray();
        }

        public static ITypeImplementation? TryImplement(TypeDefinition typeDefinition, IReadOnlyDictionary<string, TypeDefinition> typeDefinitions,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            var isMessage = typeDefinition.GeneratedSymbol.Interfaces
                .Any(interfaceSymbol => interfaceSymbol.GetNameWithNamespace() == TypeDefinition.ProtobufIMessageFullTypeName);

            return isMessage ? new MessageImplementation(typeDefinition, typeDefinitions, typeImplementationLookup) : null;
        }

        protected void WriteMessageProperties(StringBuilder code,
            ConstructionContext context, string source, string destination, ITypeImplementation typeImplementation, bool toProto)
        {
            if (!(typeImplementation is MessageImplementation messageImplementation)) return;

            foreach (var propertyDefinition in messageImplementation.PropertyDefinitions)
                TypeImplementationLookup[propertyDefinition.Definition.FullNameWithNamespace]
                    .WriteAssignment(
                        code, context, $@"{source}.{propertyDefinition.Name}", $@"{destination}.{propertyDefinition.Name}", propertyDefinition.ExplicitlyNullable, toProto);
        }

        protected class PropertyDefinition
        {
            public PropertyDefinition(string name, TypeDefinition definition, bool explicitlyNullable)
            {
                Name = name;
                Definition = definition;
                ExplicitlyNullable = explicitlyNullable;
            }

            public string Name { get; }
            public bool ExplicitlyNullable { get; }
            public TypeDefinition Definition { get; }
        }
    }
}
