using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class FlowwUserTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww";
        
        private static readonly GuidTypeOverride GuidTypeOverride = new();
        private static readonly IdentityTypeOverride IdentityTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.FlowwUser";
        
        public string FlowwNameWithNamespace => "Floww.Libraries.Global.Users.Identity.FlowwUser";

        public string ConverterName => "FlowwUserConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine($@"UserId = {GuidTypeOverride.ConverterNameWithNamespace}.Transform(source.UserId),");
                    code.AppendLine(@"ReportingCurrencyCode = source.ReportingCurrencyCode,");
                    code.AppendLine($@"Identities = {{ global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(source.Identities, {IdentityTypeOverride.ConverterNameWithNamespace}.Transform)) }},");
                    code.AppendLine($@"CurrentIdentity = {IdentityTypeOverride.ConverterNameWithNamespace}.Transform(source.CurrentIdentity),");
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
                    code.AppendLine($@"UserId = {GuidTypeOverride.ConverterNameWithNamespace}.Transform(source.UserId),");
                    code.AppendLine(@"ReportingCurrencyCode = source.ReportingCurrencyCode,");
                    code.AppendLine($@"Identities = global::System.Linq.Enumerable.ToList(global::System.Linq.Enumerable.Select(source.Identities, {IdentityTypeOverride.ConverterNameWithNamespace}.Transform)),");
                    code.AppendLine($@"CurrentIdentity = {IdentityTypeOverride.ConverterNameWithNamespace}.Transform(source.CurrentIdentity),");
                });
            });
        }
    }
}
