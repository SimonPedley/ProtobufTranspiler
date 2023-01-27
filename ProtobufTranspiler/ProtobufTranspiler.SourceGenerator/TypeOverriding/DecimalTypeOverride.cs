using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class DecimalTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Common";
        
        public string ProtoNameWithNamespace => $"{Namespace}.Decimal";
        
        public string FlowwNameWithNamespace => "System.Decimal";

        public string ConverterName => "DecimalConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AppendLine($@"const {FlowwNameWithNamespace} nanoFactor = 1_000_000_000m;");
            
            code.AppendLine($@"var units = {FlowwNameWithNamespace}.ToInt64(source);");
            code.AppendLine($@"var nanos = {FlowwNameWithNamespace}.ToInt32((source - units) * nanoFactor);");
            
            code.AsStatement(() =>
            {
                code.AppendLine($@"return new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"Units = units,");
                    code.AppendLine(@"Nanos = nanos");
                });
            });
        }

        public void BuildProtoToTargetAssignment(StringBuilder code)
        {
            code.AppendLine($@"const {FlowwNameWithNamespace} nanoFactor = 1_000_000_000m;");
            
            code.AppendLine(@"return source.Units + source.Nanos / nanoFactor;");
        }
    }
}
