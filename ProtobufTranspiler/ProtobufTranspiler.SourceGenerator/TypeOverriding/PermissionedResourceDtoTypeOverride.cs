using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class PermissionedResourceDtoTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww";
        
        public string ProtoNameWithNamespace => $"{Namespace}.PermissionedResourceDto";
        
        public string FlowwNameWithNamespace => "Floww.Libraries.Permissions.Api.Permissions.PermissionedResourceDto";

        public string ConverterName => "PermissionedResourceDtoConverter";
        
        public string ConverterNameWithNamespace => $"{Namespace}.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"Type = source.Type,");
                    code.AppendLine(@"Name = source.Name");
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
                    code.AppendLine(@"Type = source.Type,");
                    code.AppendLine(@"Name = source.Name");
                });
            });
        }
    }
}
