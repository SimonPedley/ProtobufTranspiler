using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Building.ServiceBuilding
{
    public static class ReceiverBuilder
    {
        /// <summary>
        /// Builds the receiver wrapper for the specified service
        /// </summary>
        /// <param name="context"></param>
        /// <param name="definition">The service definition to build the receiver for</param>
        public static GeneratedSource Build(ConstructionContext context, ServiceDefinition definition)
        {
            var code = new StringBuilder();
            code.DeclareNamespace(definition.Namespace, () =>
            {
                // Inherit the proto generated base class
                code.DeclareClass(
                    definition.ReceiverName,
                    isPublic: false,
                    inherits: definition.RpcBaseName,
                    content: () =>
                    {
                        // Pulls in the contract interface so the methods can call it easily
                        code.AppendLine(@$"private readonly {definition.ContractName} _rpc;");
                        code.AppendLine($@"public {definition.ReceiverName}({definition.ContractName} rpc) => _rpc = rpc;");

                        foreach (var method in definition.Methods) BuildMethod(code, context, method.Key, method.Value);
                    });
            });

            return new GeneratedSource($"{definition.Namespace}.{definition.ReceiverName}.receiver.cs", code.ToString());
        }

        /// <summary>
        /// Builds a method of the service, including conversions to and from proto types and target types
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="methodName">The name of the method being called</param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        private static void BuildMethod(StringBuilder code, ConstructionContext context, string methodName, ServiceMethodDefinition serviceMethodDefinition)
        {
            code.DeclareMethod(
                methodName,
                modifiers: "override async",
                parameters: new[]
                {
                    $@"{serviceMethodDefinition.Request?.Protobuf.FullNameWithNamespace ?? ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace} request",
                    @"global::Grpc.Core.ServerCallContext context"
                },
                returns:
                $@"global::System.Threading.Tasks.Task<{serviceMethodDefinition.Response?.Protobuf.FullNameWithNamespace ?? ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace}>",
                content: () =>
                {
                    if (serviceMethodDefinition.Response != null) code.Append(@"return ");

                    code.AsStatement(() =>
                    {
                        // Call FlowwGrpc.Receive to handle errors and common code with grpc.
                        code.Append(@"await global::Floww.Libraries.Grpc.FlowwGrpc.Receive");
                        code.WrapParenthesisAround(() =>
                        {
                            BuildHandlerCaller(code, context, methodName, serviceMethodDefinition);
                            BuildResponseHandling(code, context, serviceMethodDefinition);
                        });
                    });

                    if (serviceMethodDefinition.Response is null) code.AppendLine($@"return new {ProtobufServiceAnalyser.ProtoEmptyTypeNameWithNamespace}();");
                });
        }

        /// <summary>
        /// Calls the contract interface handler, converting proto type parameters to their target type counterparts
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="methodName">The name of the method being called</param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        private static void BuildHandlerCaller(StringBuilder code, ConstructionContext context, string methodName, ServiceMethodDefinition serviceMethodDefinition)
        {
            code.Append(@"context, (currentUser, deadline, cancellationToken) => ");
            code.WrapBracesAround(() =>
            {
                string? responseVariableName = null;

                if (serviceMethodDefinition.Request != null)
                {
                    responseVariableName = code.DeclareVariable(context, serviceMethodDefinition.Request.GetFlowwFullTypeNameWithNamespace());
                    serviceMethodDefinition.Request.WriteAssignment(code, context, "request", responseVariableName, false, false);
                }

                code.AsStatement(() =>
                {
                    code.Append(@$"return _rpc.{methodName}");
                    code.WrapParenthesisAround(() =>
                    {
                        if (responseVariableName != null) code.Append($@"{responseVariableName}, ");
                        code.Append(@"currentUser");
                        code.Append(", cancellationToken: cancellationToken");
                    });
                });
            });
        }

        /// <summary>
        /// Converts the response message from a target type to it's proto type counterpart
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="serviceMethodDefinition">The definition of the method being called</param>
        private static void BuildResponseHandling(StringBuilder code, ConstructionContext context, ServiceMethodDefinition serviceMethodDefinition)
        {
            if (serviceMethodDefinition.Response is null) return;

            code.Append(", response => ");
            code.WrapBracesAround(() =>
            {
                var responseVariableName = code.DeclareVariable(context, serviceMethodDefinition.Response.Protobuf.FullNameWithNamespace);
                serviceMethodDefinition.Response.WriteAssignment(code, context, "response", responseVariableName, false, true);
                code.AppendLine($"return {responseVariableName};");
            });
        }
    }
}
