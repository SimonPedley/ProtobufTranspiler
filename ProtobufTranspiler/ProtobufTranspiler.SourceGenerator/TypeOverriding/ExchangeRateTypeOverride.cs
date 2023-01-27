using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class ExchangeRateTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww";
        
        private static readonly DecimalTypeOverride DecimalTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.ExchangeRate";
        
        public string FlowwNameWithNamespace => "Floww.Libraries.Common.Entities.ExchangeRate";

        public string ConverterName => "ExchangeRateConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"SourceCurrencyCode = source.SourceCurrencyCode,");
                    code.AppendLine(@"TargetCurrencyCode = source.TargetCurrencyCode ,");
                    code.AppendLine($@"Rate = {DecimalTypeOverride.ConverterNameWithNamespace}.Transform(source.Rate)");
                });
            });
        }

        public void BuildProtoToTargetAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {FlowwNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"SourceCurrencyCode = source.SourceCurrencyCode,");
                    code.AppendLine(@"TargetCurrencyCode = source.TargetCurrencyCode ,");
                    code.AppendLine($@"Rate = {DecimalTypeOverride.ConverterNameWithNamespace}.Transform(source.Rate)");
                });
            });
        }
    }
}
