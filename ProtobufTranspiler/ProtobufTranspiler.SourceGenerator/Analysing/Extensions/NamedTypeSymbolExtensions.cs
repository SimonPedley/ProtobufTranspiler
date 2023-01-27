using Microsoft.CodeAnalysis;

namespace ProtobufTranspiler.SourceGenerator.Analysing.Extensions
{
    public static class NamedTypeSymbolExtensions
    {
        public static string GetNameWithNamespace(this ITypeSymbol typeSymbol) => $@"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}";
    }
}