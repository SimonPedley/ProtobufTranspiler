using ProtobufTranspiler.SourceGenerator.Building.ServiceBuilding;

namespace ProtobufTranspiler.SourceGenerator.Building
{
    public static class ServiceBuilder
    {
        /// <summary>
        /// Builds the interface, caller and receiver wrappers around the proto service.
        /// </summary>
        /// <param name="context"></param>
        public static IEnumerable<GeneratedSource> Build(ConstructionContext context)
        {
            foreach (var serviceDefinition in context.ServiceDefinitions)
            {
                yield return ContractBuilder.Build(serviceDefinition);
                yield return CallerBuilder.Build(context, serviceDefinition);
                yield return ReceiverBuilder.Build(context, serviceDefinition);
            }
        }
    }
}
