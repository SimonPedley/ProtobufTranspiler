using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class GuidNullableTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        private static readonly GuidTypeOverride GuidTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.GuidNullable";

        public string FlowwNameWithNamespace => "System.Guid?";

        public string ConverterName => "GuidNullableConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code) =>
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"HasValue = source != null,");
                    code.AppendLine($@"Value = {GuidTypeOverride.ConverterNameWithNamespace}.Transform(source ?? default({GuidTypeOverride.FlowwNameWithNamespace}))");
                });
            });

        public void BuildProtoToTargetAssignment(StringBuilder code) =>
            code.AppendLine($@"return source != null && source.HasValue ? ({FlowwNameWithNamespace}){GuidTypeOverride.ConverterNameWithNamespace}.Transform(source.Value) : ({FlowwNameWithNamespace})null;");
    }
}
