using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DecimalNullableTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly DecimalTypeOverride DecimalTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.DecimalNullable";
        
        public string FlowwNameWithNamespace => "System.Decimal?";

        public string ConverterName => "DecimalNullableConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"HasValue = source != null,");
                    code.AppendLine($@"Value = {DecimalTypeOverride.ConverterNameWithNamespace}.Transform(source ?? default({DecimalTypeOverride.FlowwNameWithNamespace}))");
                });
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) =>
            code.AppendLine($@"return source != null && source.HasValue ? ({FlowwNameWithNamespace}){DecimalTypeOverride.ConverterNameWithNamespace}.Transform(source.Value) : ({FlowwNameWithNamespace})null;");
    }
}
