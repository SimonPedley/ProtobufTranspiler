namespace ProtobufTranspiler.SourceGenerator.Analysing.Model
{
    /// <summary>
    /// Meta for a proto service
    /// </summary>
    public class ServiceDefinition
    {
        /// <summary>
        /// The name of this service
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The namespace this service resides
        /// </summary>
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// The name of the proto generated client class
        /// </summary>
        public string RpcClientName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the proto generated base class
        /// </summary>
        public string RpcBaseName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the contract interface to be generated for this service
        /// </summary>
        public string ContractName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the caller class to be generated for this service
        /// </summary>
        public string CallerName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the receiver class to be generated for this service
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of methods that this service has
        /// </summary>
        public IReadOnlyDictionary<string, ServiceMethodDefinition> Methods { get; set; } = new Dictionary<string, ServiceMethodDefinition>(0);
    }
}
