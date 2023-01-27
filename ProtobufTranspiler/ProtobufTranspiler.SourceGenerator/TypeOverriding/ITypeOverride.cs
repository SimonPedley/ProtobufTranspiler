using System.Text;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public interface ITypeOverride
    {
        /// <summary>
        /// The full name, including namespace, of the proto generated type
        /// </summary>
        public string ProtoNameWithNamespace { get; }
        
        /// <summary>
        /// The full name, including namespace, of the desired replacement type
        /// </summary>
        public string FlowwNameWithNamespace { get; }
        
        /// <summary>
        /// The name of the generated converter
        /// </summary>
        public string ConverterName { get; }
        
        /// <summary>
        /// The name of the generated converter, including it's namespace
        /// </summary>
        public string ConverterNameWithNamespace { get; }

        /// <summary>
        /// The code generation for transforming from the target type to the proto type.
        /// The target variable is named source.
        /// </summary>
        /// <param name="code">The code string builder</param>
        public void BuildTargetToProtoAssignment(StringBuilder code);
        
        /// <summary>
        /// The code generation for transforming from the proto type to the target type.
        /// The proto variable is named source.
        /// </summary>
        /// <param name="code">The code string builder</param>
        public void BuildProtoToTargetAssignment(StringBuilder code);
    }
}
