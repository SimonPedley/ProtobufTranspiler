using Microsoft.CodeAnalysis;
using ProtobufTranspiler.SourceGenerator.Analysing.Extensions;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Implementations;

namespace ProtobufTranspiler.SourceGenerator.Analysing
{
    public class ProtobufServiceAnalyser
    {
        public const string ProtoEmptyTypeNameWithNamespace = @"Proto.Common.Empty";
        public const string ProtoBindServiceMethodAttributeNameWithNamespace = @"Grpc.Core.BindServiceMethodAttribute";

        public static IEnumerable<ServiceDefinition> Analyse(GenerationContext context, IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            return context.Compilation.GlobalNamespace
                .GetNamespaceMembers()
                .First(namespaceSymbol => namespaceSymbol.Name == "Proto")
                .GetAllTypesRecursively()
                .Where(typeSymbol =>
                    typeSymbol.TypeKind == TypeKind.Class &&
                    typeSymbol
                        .GetAttributes()
                        .Any(attributeData => attributeData.AttributeClass?.GetNameWithNamespace() == ProtoBindServiceMethodAttributeNameWithNamespace))
                .Where(symbol => symbol.ContainingAssembly.ToString() == context.Compilation.Assembly.ToString())
                .Select(typeSymbol =>
                {
                    var serviceNamespace = typeSymbol.ContainingNamespace.ToString();
                    var serviceName = typeSymbol.Name.Substring(0, typeSymbol.Name.Length - 4);

                    return new ServiceDefinition
                    {
                        Name = serviceName,
                        Namespace = serviceNamespace,
                        RpcClientName = $"global::{serviceNamespace}.{serviceName}.{serviceName}Client",
                        RpcBaseName = $"global::{serviceNamespace}.{serviceName}.{serviceName}Base",
                        ContractName = $"I{serviceName}",
                        CallerName = $"{serviceName}Caller",
                        ReceiverName = $"{serviceName}Receiver",
                        Methods = typeSymbol
                            .GetMembers()
                            .OfType<IMethodSymbol>()
                            .Where(methodSymbol => !methodSymbol.IsImplicitlyDeclared && methodSymbol.MethodKind == MethodKind.Ordinary)
                            .ToDictionary(
                                methodSymbol => methodSymbol.Name,
                                methodSymbol => AnalyseMethod(methodSymbol, typeImplementationLookup))
                    };
                });
        }

        private static ServiceMethodDefinition AnalyseMethod(IMethodSymbol methodSymbol, IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            var requestTypeNameWithNamespace = methodSymbol.Parameters.First(parameterSymbol => parameterSymbol.Name != "context").Type.ToString();
            var returnTypeNameWithNamespace = ((INamedTypeSymbol)methodSymbol.ReturnType).TypeArguments.First().ToString();
            
            return new ServiceMethodDefinition
            {
                Request = requestTypeNameWithNamespace != ProtoEmptyTypeNameWithNamespace
                    ? typeImplementationLookup[requestTypeNameWithNamespace]
                    : null,
                Response = returnTypeNameWithNamespace != ProtoEmptyTypeNameWithNamespace
                    ? typeImplementationLookup[returnTypeNameWithNamespace]
                    : null
            };
        }
    }
}
