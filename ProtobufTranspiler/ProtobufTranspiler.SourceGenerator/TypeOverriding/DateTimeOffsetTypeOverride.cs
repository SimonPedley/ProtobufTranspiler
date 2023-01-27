using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DateTimeOffsetTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly TimestampTypeOverride TimestampTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.DateTimeOffset";
        
        public string FlowwNameWithNamespace => "System.DateTimeOffset";

        public string ConverterName => "DateTimeOffsetConverter";

        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() => code.AppendLine($@"Value = {TimestampTypeOverride.ConverterNameWithNamespace}.Transform(source)"));
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) =>
            code.AppendLine($@"return {TimestampTypeOverride.ConverterNameWithNamespace}.Transform(source.Value);");
    }
}
