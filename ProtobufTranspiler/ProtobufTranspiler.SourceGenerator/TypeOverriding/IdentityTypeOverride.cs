using System.Text;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class IdentityTypeTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww.IdentityType";
        
        public string ProtoNameWithNamespace => $"{Namespace}.IdentityType";

        public string FlowwNameWithNamespace => "Floww.Libraries.Global.Users.Identity.IdentityType";

        public string ConverterName => "IdentityTypeConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AppendLine($@"return ({ProtoNameWithNamespace})(System.Int32)source;");
        }

        public void BuildProtoToTargetAssignment(StringBuilder code)
        {
            code.AppendLine($@"return ({FlowwNameWithNamespace})(System.Int32)source;");
        }
    }
}
