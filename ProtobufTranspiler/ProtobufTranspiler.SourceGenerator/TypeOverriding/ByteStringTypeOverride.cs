using System.Text;
using ProtobufTranspiler.SourceGenerator.Implementations;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class ByteStringTypeOverride : ITypeOverride
    {
        public const string Namespace = "Google.Protobuf";
        
        public string ProtoNameWithNamespace => $"{Namespace}.ByteString";

        public string FlowwNameWithNamespace => $@"{CollectionImplementation.FlowwEnumerationTypeNameWithNamespace}<System.Byte>";

        public string ConverterName => "ByteStringConverter";
        
        public string ConverterNameWithNamespace => $"{Namespace}.{ConverterName}";
        
        public void BuildTargetToProtoAssignment(StringBuilder code) => code.AppendLine($@"return {ProtoNameWithNamespace}.CopyFrom(System.Linq.Enumerable.ToArray(source));");

        public void BuildProtoToTargetAssignment(StringBuilder code) => code.AppendLine(@"return System.Linq.Enumerable.ToList(source.ToByteArray());");
    }
}
