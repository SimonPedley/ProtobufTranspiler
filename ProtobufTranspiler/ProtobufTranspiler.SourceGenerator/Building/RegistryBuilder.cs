using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.Building
{
    public class RegistryBuilder
    {
        /// <summary>
        /// Builds static extension methods to register GRPC functions for both calling and receiving services to register the generated code.
        /// </summary>
        /// <param name="context">The build compilation</param>
        public static GeneratedSource Build(ConstructionContext context)
        {
            // Caller code is registered via service registry and requires a service endpoint so that the caller knows the address of the receiver host
            var code = new StringBuilder();

            var simplifiedProjectName = context.Generation.Compilation.GlobalNamespace.ToString().Replace(".", string.Empty);

            code.DeclareNamespace(context.Generation.Compilation.GlobalNamespace.ToString(), () =>
            {
                code.DeclareClass(
                    simplifiedProjectName,
                    isStatic: true,
                    content: () =>
                {
                    code.DeclareMethod(
                        @$"Add{simplifiedProjectName}Client",
                        modifiers: "static",
                        parameters: new[]
                        {
                            @"this global::Lamar.ServiceRegistry registry",
                            @"global::System.String serviceEndpoint"
                        },
                        content: () =>
                        {
                            // Used to manage the connection
                            code.AppendLine("var grpcChannelControl = new global::Floww.Libraries.Grpc.GrpcChannelControl(serviceEndpoint);");
                            // Registered so that GrpcChannelControls can be stopped gracefully when the service shuts down
                            code.AppendLine("registry.For<global::Floww.Libraries.Grpc.GrpcChannelControl>().Use(grpcChannelControl);");
                            
                            foreach (var definition in context.ServiceDefinitions)
                            {
                                // Registers the proto generated client with the endpoint channel
                                code.AppendLine($"registry.For<{definition.RpcClientName}>().Use(new {definition.RpcClientName}(grpcChannelControl.Channel));");
                                // Registers the contract interface to the caller class
                                code.AppendLine($"registry.For<{definition.ContractName}>().Use<{definition.CallerName}>();");
                            }
                        });
                });
            });

            return new GeneratedSource($"{context.Generation.Compilation.GlobalNamespace}.{simplifiedProjectName}.registry.cs", code.ToString());
        }
    }
}
