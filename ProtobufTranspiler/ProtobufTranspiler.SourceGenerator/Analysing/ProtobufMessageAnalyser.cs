using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing.Extensions;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Implementations;

namespace ProtobufTranspiler.SourceGenerator.Analysing
{
    public class ProtobufMessageAnalyser
    {
        public static IReadOnlyDictionary<string, ITypeImplementation> Analyse(GenerationContext context)
        {
            var typeDefinitionLookup = context.Compilation.GlobalNamespace
                .GetNamespaceMembers()
                .First(namespaceSymbol => namespaceSymbol.Name == "Proto")
                .GetAllTypesRecursively()
                .Where(typeSymbol =>
                    typeSymbol.TypeKind == TypeKind.Class && typeSymbol.Interfaces.Any(interfaceSymbol => interfaceSymbol.GetNameWithNamespace() == MessageImplementation.ProtobufIMessageTypeNameWithNamespace) ||
                    typeSymbol.TypeKind == TypeKind.Enum)
                .Select(typeSymbol => new TypeDefinition(typeSymbol, typeSymbol.ContainingAssembly.ToString() != context.Compilation.Assembly.ToString()))
                .ToDictionary(typeDefinition => typeDefinition.FullNameWithNamespace, messageDefinition => messageDefinition);

            foreach (var typeDefinition in typeDefinitionLookup.Values.ToArray()) RecursivelyFindReferencedTypes(typeDefinition, typeDefinitionLookup);

            var typeImplementationLookup = new Dictionary<string, ITypeImplementation>(typeDefinitionLookup.Count);
            
            foreach (var typeDefinition in typeDefinitionLookup.Values)
                typeImplementationLookup.Add(
                    typeDefinition.FullNameWithNamespace,
                    OverriddenImplementation.TryImplement(typeDefinition) ??
                    EnumImplementation.TryImplement(typeDefinition) ??
                    PaginationImplementation.TryImplement(typeDefinition, typeDefinitionLookup, typeImplementationLookup) ??
                    CollectionImplementation.TryImplement(typeDefinition, typeDefinitionLookup, typeImplementationLookup) ??
                    MessageImplementation.TryImplement(typeDefinition, typeDefinitionLookup, typeImplementationLookup) ??
                    AsIsImplementation.Implement(typeDefinition));

            return typeImplementationLookup;
        }

        private static void RecursivelyFindReferencedTypes(TypeDefinition typeDefinition, IDictionary<string, TypeDefinition> typeDefinitionLookup)
        {
            if (typeDefinition.GeneratedSymbol.TypeKind == TypeKind.Enum) return;

            var propertyTypeSymbols = typeDefinition.GeneratedSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(symbol => !symbol.IsReadOnly || symbol.Type.GetNameWithNamespace() == CollectionImplementation.ProtobufRepeatedFieldTypeNameWithNamespace)
                .Select(propertySymbol => propertySymbol.Type);
            
            foreach (var typeSymbol in propertyTypeSymbols)
            {
                var propertyTypeDefinition = new TypeDefinition((INamedTypeSymbol)typeSymbol, true);

                if (typeDefinitionLookup.ContainsKey(propertyTypeDefinition.FullNameWithNamespace)) continue;
                
                typeDefinitionLookup.Add(propertyTypeDefinition.FullNameWithNamespace, propertyTypeDefinition);
                RecursivelyFindReferencedTypes(propertyTypeDefinition, typeDefinitionLookup);
            }
        }
    }
}
