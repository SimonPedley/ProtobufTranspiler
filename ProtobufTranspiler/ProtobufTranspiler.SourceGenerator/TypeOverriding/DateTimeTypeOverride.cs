using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DateTimeTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly TimestampTypeOverride TimestampTypeOverride = new();
        
        public string ProtoNameWithNamespace => $"{Namespace}.DateTime";
        
        public string FlowwNameWithNamespace => "System.DateTime";

        public string ConverterName => "DateTimeConverter";

        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() => code.AppendLine($@"Value = {TimestampTypeOverride.ProtoNameWithNamespace}.FromDateTime(source.ToUniversalTime())"));
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) => code.AppendLine(@"return source.Value.ToDateTime();");
    }
}
