using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class GuidTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        public string ProtoNameWithNamespace => $"{Namespace}.Guid";
        
        public string FlowwNameWithNamespace => "System.Guid";

        public string ConverterName => "GuidConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() => code.AppendLine(@"Value = source.ToString(""D"")"));
            });
        }

        public void BuildProtoToTargetAssignment(StringBuilder code) => code.AppendLine(@"return System.Guid.ParseExact(source.Value, ""D"");");
    }
}
