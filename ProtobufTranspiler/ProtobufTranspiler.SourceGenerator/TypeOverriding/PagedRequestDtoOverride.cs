using System.Text;
using ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions;

namespace ProtobufTranspiler.SourceGenerator.TypeOverriding
{
    public class PagedRequestDtoTypeOverride : ITypeOverride
    {
        public const string Namespace = "Proto.Floww";
        
        public string ProtoNameWithNamespace => $"{Namespace}.PagedRequestDto";
        
        public string FlowwNameWithNamespace => "Floww.Libraries.Common.PagedRequestDto";

        public string ConverterName => "PagedRequestDtoConverter";
        
        public string ConverterNameWithNamespace => $@"{Namespace}.Generated.{ConverterName}";

        public void BuildTargetToProtoAssignment(StringBuilder code)
        {
            code.AsStatement(() =>
            {
                code.Append($@"return source == null ? null : new {ProtoNameWithNamespace}");
                code.WrapBracesAround(() =>
                {
                    code.AppendLine(@"PageNumber = source.PageNumber,");
                    code.AppendLine(@"PageSize = source.PageSize,");
                    code.AppendLine(@"Sort = source.Sort,");
                    code.AppendLine(@"SortAscending = source.SortAscending");
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
                    code.AppendLine(@"PageNumber = source.PageNumber,");
                    code.AppendLine(@"PageSize = source.PageSize,");
                    code.AppendLine(@"Sort = source.Sort,");
                    code.AppendLine(@"SortAscending = source.SortAscending");
                });
            });
        }
    }
}
