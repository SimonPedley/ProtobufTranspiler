using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;
using ProtobufTranspiler.SourceGenerator.Implementations;
using ProtobufTranspiler.SourceGenerator.TypeOverriding;

namespace ProtobufTranspiler.SourceGenerator.Building.ServiceBuilding
{
    public static class CallerBuilder
    {
        private static readonly FlowwUserTypeOverride FlowwUserTypeOverride = new();

        /// <summary>
        /// Builds the caller class for a service definition.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceDefinition">The service to build off</param>
        public static GeneratedSource Build(ConstructionContext context, ServiceDefinition serviceDefinition)
        {
            var code = new StringBuilder();

            code.DeclareNamespace(serviceDefinition.Namespace, () =>
            {
                code.DeclareClass(serviceDefinition.CallerName, isPublic: false, content: () =>
                {
                    // Pull in the proto generated service client into a field so that we can call it from the methods
                    code.AppendLine(@$"private readonly {serviceDefinition.RpcClientName} _rpcClient;");
                    code.AppendLine($@"public {serviceDefinition.CallerName}({serviceDefinition.RpcClientName} rpcClient) => _rpcClient = rpcClient;");

                    foreach (var method in serviceDefinition.Methods) BuildMethod(code, context, method.Key, method.Value, context.TypeImplementations);
                }, inherits: serviceDefinition.ContractName);
            });

            return new GeneratedSource($"{serviceDefinition.Namespace}.{serviceDefinition.CallerName}.caller.cs", code.ToString());
        }

        /// <summary>
        /// Implements the call translation from the target method to the protobuf method, converting the request and response messages respectfully.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="methodName">The name of the method to be called</param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        /// <param name="typeImplementationLookup"> definitions to use while building</param>
        private static void BuildMethod(StringBuilder code, ConstructionContext context, string methodName, ServiceMethodDefinition serviceMethodDefinition,
            IReadOnlyDictionary<string, ITypeImplementation> typeImplementationLookup)
        {
            var parameters = new List<string>();

            if (serviceMethodDefinition.Request != null)
            {
                var requestType = serviceMethodDefinition.Request.GetFlowwFullTypeNameWithNamespace();
                parameters.Add($@"{requestType} request");
            }

            var flowwTypeNameWithNamespace = typeImplementationLookup[FlowwUserTypeOverride.ProtoNameWithNamespace].GetFlowwFullTypeNameWithNamespace(true);
            parameters.Add($@"{flowwTypeNameWithNamespace} currentUser");
            parameters.Add(@"System.Threading.CancellationToken cancellationToken = default");

            var returnType = (serviceMethodDefinition.Response != null
                ? $@"<{serviceMethodDefinition.Response.GetFlowwFullTypeNameWithNamespace()}>"
                : string.Empty);

            code.DeclareMethod(
                methodName,
                returns: $"global::System.Threading.Tasks.Task<Floww.Libraries.Common.ServiceResult{returnType}>",
                parameters: parameters,
                content: () =>
                {
                    code.AsStatement(() =>
                    {
                        // We use the FlowwGrpc.Send static function to handle Grpc errors.
                        code.Append(@"return global::Floww.Libraries.Grpc.FlowwGrpc.Send");
                        code.WrapParenthesisAround(() =>
                        {
                            // We pass FlowwGrpc.Send an async function that calls the correct method on the client.
                            code.Append(@"async (metadata, deadline, cancellationToken) =>");
                            code.WrapBracesAround(() =>
                            {
                                BuildCallToRpcClient(code, context, methodName, serviceMethodDefinition);
                                BuildResponseHandling(code, context, serviceMethodDefinition);
                            });
                            code.Append(", currentUser");
                            code.Append(", cancellationToken: cancellationToken");
                        });
                    });
                });
        }

        /// <summary>
        /// Calls the proto generated RPC client, converting target type parameters to their proto type counterparts
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="methodName">The name of the method to be called</param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        private static void BuildCallToRpcClient(StringBuilder code, ConstructionContext context, string methodName, ServiceMethodDefinition serviceMethodDefinition)
        {
            var requestVariableName = code.DeclareVariable(context, serviceMethodDefinition.Request?.Protobuf.FullNameWithNamespace ?? ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace);

            if (serviceMethodDefinition.Request != null) serviceMethodDefinition.Request.WriteAssignment(code, context, "request", requestVariableName, false, true);
            else code.AppendLine($@"{requestVariableName} = new {ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace}();");

            code.AsStatement(() =>
            {
                code.Append(@$"var response = await _rpcClient.{methodName}Async");
                code.WrapParenthesisAround(() =>
                {
                    code.Append($@"{requestVariableName}, metadata");
                    code.Append(@", cancellationToken: cancellationToken");
                });
            });
        }

        /// <summary>
        /// Converts the response message from a proto type to it's target type counterpart
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        private static void BuildResponseHandling(StringBuilder code, ConstructionContext context, ServiceMethodDefinition serviceMethodDefinition)
        {
            if (serviceMethodDefinition.Response is null) return;

            var responseVariableName = code.DeclareVariable(context, serviceMethodDefinition.Response.GetFlowwFullTypeNameWithNamespace());
            serviceMethodDefinition.Response.WriteAssignment(code, context, "response", responseVariableName, false, false);
            code.AppendLine($"return {responseVariableName};");
        }
    }
}
