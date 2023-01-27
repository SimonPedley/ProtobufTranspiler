using ProtobufTranspiler.SourceGenerator.Implementations;

namespace ProtobufTranspiler.SourceGenerator.Analysing.Model
{
    /// <summary>
    /// Metadata of a service method
    /// </summary>
    public class ServiceMethodDefinition
    {
        /// <summary>
        /// The metadata of the request message for this method.
        /// </summary>
        public ITypeImplementation? Request { get; set; } = null;
        
        /// <summary>
        /// The metadata of the response message for this method.
        /// </summary>
        public ITypeImplementation? Response { get; set; } = null;
    }
}