using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class IdentityTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww";
        
        private static readonly GuidTypeOverride GuidTypeOverride = new();
        private static readonly IdentityTypeTypeOverride IdentityTypeTypeOverride = new();

        public string ProtoNameWithNamespace => $"{Namespace}.Identity";
        
        public string FlowwNameWithNamespace => "Floww.Libraries.Global.Users.Identity.Identity";

        public string ConverterName => "IdentityConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {

            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine($@"IdentityId = {GuidTypeOverride.ConverterNameWithNamespace}.Transform(source.IdentityId),");
                    code.AppendLine($@"Type = {IdentityTypeTypeOverride.ConverterNameWithNamespace}.Transform(source.Type),");
                    code.AppendLine(@"Roles = { global::System.Linq.Enumerable.ToList(source.Roles ?? global::System.Linq.Enumerable.Empty<System.String>()) },");
                    code.AppendLine(@"Scope = source.Scope");
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
                    code.AppendLine($@"IdentityId = {GuidTypeOverride.ConverterNameWithNamespace}.Transform(source.IdentityId),");
                    code.AppendLine($@"Type = {IdentityTypeTypeOverride.ConverterNameWithNamespace}.Transform(source.Type),");
                    code.AppendLine(@"Roles = global::System.Linq.Enumerable.ToList(source.Roles),");
                    code.AppendLine(@"Scope = source.Scope");
                });
            });
        }
    }
}
