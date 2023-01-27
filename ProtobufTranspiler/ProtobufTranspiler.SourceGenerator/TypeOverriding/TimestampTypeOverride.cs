using System.Text;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class TimestampTypeOverride : ITypeOverride
    {
        public const string Namespace = "Google.Protobuf.WellKnownTypes";
        
        public string ProtoNameWithNamespace => $"{Namespace}.Timestamp";
        
        public string FlowwNameWithNamespace => "System.DateTimeOffset";

        public string ConverterName => "TimestampConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) => code.AppendLine($@"return {ProtoNameWithNamespace}.FromDateTimeOffset(source);");

        public void BuildProtoToTargetAssignment(StringBuilder code) => code.AppendLine(@"return source.ToDateTimeOffset();");
    }
}
