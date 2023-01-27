using System.Text;

namespace ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions
{
    public static class GenericCodeExtensions
    {
        /// <summary>
        /// Wraps curley braces around the code written within the action.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="content">The action to write the code written within the braces</param>
        public static void WrapBracesAround(this StringBuilder code, Action content)
        {
            code.AppendLine("{");
            content();
            code.Append('}');
        }

        /// <summary>
        /// Wraps parenthesis braces around the code written within the action.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="content">The action to write the code written within the parenthesis</param>
        public static void WrapParenthesisAround(this StringBuilder code, Action content)
        {
            code.Append('(');
            content();
            code.Append(')');
        }

        /// <summary>
        /// Symbolises that all code within the content action is part of a single statement and ensures there is a final semi-colon.
        /// Note: Used for when the building of a statement is quite large and complicated. This ensures that we can see all code involved in a complex statement and also ensures there's always a semi-colon at the end.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="content">The action writing the content of the statement.</param>
        public static void AsStatement(this StringBuilder code, Action content)
        {
            content();
            code.AppendLine(";");
        }
    }
}