using System.Text;
using ProtobufTranspiler.SourceGenerator.Analysing.Model;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;
using ProtobufTranspiler.SourceGenerator.TypeOverriding;

namespace ProtobufTranspiler.SourceGenerator.Building.ServiceBuilding
{
    public static class ContractBuilder
    {
        private static readonly FlowwUserTypeOverride FlowwUserTypeOverride = new();

        /// <summary>
        /// Builds the contract interface used by Floww code to call and receive this RPC.
        /// </summary>
        /// <param name="definition">The service definition to build the contract for</param>
        public static GeneratedSource Build(ServiceDefinition definition)
        {
            var code = new StringBuilder();

            code.DeclareNamespace(definition.Namespace, () =>
            {
                code.DeclareInterface(definition.ContractName, content: () =>
                {
                    foreach (var method in definition.Methods)
                    {
                        var parameters = new List<string>();
                        
                        if (method.Value.Request != null)
                        {
                            var requestType = method.Value.Request.GetFlowwFullTypeNameWithNamespace();
                            parameters.Add($@"{requestType} request");
                        }
                        
                        parameters.Add($@"{FlowwUserTypeOverride.FlowwNameWithNamespace} currentUser");
                        parameters.Add(@"System.Threading.CancellationToken cancellationToken = default");

                        var returnType = method.Value.Response != null
                            ? $@"<{method.Value.Response.GetFlowwFullTypeNameWithNamespace()}>"
                            : string.Empty;
                        
                        code.DeclareMethod(
                            method.Key,
                            parameters: parameters,
                            returns: $"global::System.Threading.Tasks.Task<Floww.Libraries.Common.ServiceResult{returnType}>");
                    }
                });
            });

            return new GeneratedSource($"{definition.Namespace}.{definition.ContractName}.contract.cs", code.ToString());
        }
    }
}
