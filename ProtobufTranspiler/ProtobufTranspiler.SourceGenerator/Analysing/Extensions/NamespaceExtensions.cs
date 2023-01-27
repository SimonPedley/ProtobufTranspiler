using Microsoft.CodeAnalysis;

namespace ProtobufTranspiler.SourceGenerator.Analysing.Extensions
{
    public static class NamespaceExtensions
    {
        public static IReadOnlyCollection<INamedTypeSymbol> GetAllTypesRecursively(this INamespaceSymbol namespaceSymbol)
        {
            var typeSymbolLookup = new Dictionary<string, INamedTypeSymbol>();
            GetAllTypesRecursively(namespaceSymbol, typeSymbolLookup);
            return typeSymbolLookup.Values;
        }

        private static void GetAllTypesRecursively(INamespaceOrTypeSymbol namespaceOrTypeSymbol, IDictionary<string, INamedTypeSymbol> typeSymbolLookup)
        {
            foreach (var nestedNamespaceOrTypeSymbol in namespaceOrTypeSymbol.GetMembers().OfType<INamespaceOrTypeSymbol>())
            {
                if (nestedNamespaceOrTypeSymbol.IsType)
                {
                    var typeSymbol = (INamedTypeSymbol)nestedNamespaceOrTypeSymbol;
                    var typeNameWithNamespace = typeSymbol.ToString();
                    if (!typeSymbolLookup.ContainsKey(typeNameWithNamespace))
                    {
                        typeSymbolLookup.Add(typeNameWithNamespace, typeSymbol);
                        GetAllTypesRecursively(nestedNamespaceOrTypeSymbol, typeSymbolLookup);
                    }
                    continue;
                }
            
                GetAllTypesRecursively(nestedNamespaceOrTypeSymbol, typeSymbolLookup);
            }
        }
    }
}