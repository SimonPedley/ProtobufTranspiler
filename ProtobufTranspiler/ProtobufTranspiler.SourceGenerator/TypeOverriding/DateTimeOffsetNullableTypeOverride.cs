using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DateTimeOffsetNullableTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly DateTimeOffsetTypeOverride DateTimeOffsetTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.DateTimeOffsetNullable";
        
        public string FlowwNameWithNamespace => "System.DateTimeOffset?";

        public string ConverterName => "DateTimeOffsetNullableConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"HasValue = source != null,");
                    code.AppendLine($@"Value = {DateTimeOffsetTypeOverride.ConverterNameWithNamespace}.Transform(source ?? default({DateTimeOffsetTypeOverride.FlowwNameWithNamespace}))");
                });
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) =>
            code.AppendLine($@"return source != null && source.HasValue ? ({FlowwNameWithNamespace}){DateTimeOffsetTypeOverride.ConverterNameWithNamespace}.Transform(source.Value) : ({FlowwNameWithNamespace})null;");
    }
}
