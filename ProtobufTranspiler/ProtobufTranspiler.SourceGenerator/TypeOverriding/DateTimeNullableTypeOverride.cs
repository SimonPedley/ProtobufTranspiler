using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DateTimeNullableTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly DateTimeTypeOverride DateTimeTypeOverride = new();

        public string ProtoNameWithNamespace => $@"{Namespace}.DateTimeNullable";
        
        public string FlowwNameWithNamespace => "System.DateTime?";

        public string ConverterName => "DateTimeNullableConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"HasValue = source != null,");
                    code.AppendLine($@"Value = {DateTimeTypeOverride.ConverterNameWithNamespace}.Transform(source ?? default({DateTimeTypeOverride.FlowwNameWithNamespace}))");
                });
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) =>
            code.AppendLine($@"return source != null && source.HasValue ? ({FlowwNameWithNamespace}){DateTimeTypeOverride.ConverterNameWithNamespace}.Transform(source.Value) : ({FlowwNameWithNamespace})null;");
    }
}
