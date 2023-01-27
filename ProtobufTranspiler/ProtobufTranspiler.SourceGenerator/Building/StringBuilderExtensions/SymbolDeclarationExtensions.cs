using System.Text;

namespace ProtobufTranspiler.SourceGenerator.Building.StringBuilderExtensions
{
    public static class SymbolDeclarationExtensions
    {
        /// <summary>
        /// Writes the code declaring a namespace
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">Name of the namespace</param>
        /// <param name="content">The action to write the content of the namespace</param>
        public static void DeclareNamespace(this StringBuilder code, string name, Action content)
        {
            code.AppendLine($"namespace {name}");
            code.WrapBracesAround(content);
        }

        /// <summary>
        /// Writes the code declaring an interface.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">Name of the interface</param>
        /// <param name="content">The action to write the content of the interface</param>
        /// <param name="inherits">When specified, declares the interface to inherit the specified reference(s)</param>
        public static void DeclareInterface(this StringBuilder code, string name, Action content, string? inherits = null)
        {
            code.Append($"public interface {name}");
            if (!string.IsNullOrWhiteSpace(inherits)) code.Append($" : {inherits}");
            code.AppendLine();
            code.WrapBracesAround(content);
        }

        /// <summary>
        /// Writes the code declaring a class.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">The name of the class</param>
        /// <param name="content">The action to write the content of the interface</param>
        /// <param name="inherits">When specified, declares the class to inherit the specified reference(s)</param>
        /// <param name="isStatic">When true, declares the class as static.</param>
        /// <param name="isPublic">When true, declares the class with public visibility, otherwise it is declared as internal.</param>
        public static void DeclareClass(this StringBuilder code, string name, Action? content, string? inherits = null, bool isStatic = false, bool isPublic = true)
        {
            code.Append(isPublic ? "public" : "internal");
            if (isStatic) code.Append(" static");
            code.Append($" class {name}");
            if (!string.IsNullOrWhiteSpace(inherits)) code.Append($" : {inherits}");
            code.AppendLine();
            code.WrapBracesAround(content ?? (() => { }));
            code.AppendLine();
        }

        /// <summary>
        /// Writes the code declaring a class.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">The name of the class</param>
        /// <param name="values">The values of the enum, starting with the numeric value, followed by the key name.</param>
        /// <param name="isPublic">When true, declares the class with public visibility, otherwise it is declared as internal.</param>
        public static void DeclareEnum(this StringBuilder code, string name, IReadOnlyDictionary<string, int> values, bool isPublic = true)
        {
            code.Append(isPublic ? "public" : "internal");
            code.Append($" enum {name}");
            code.AppendLine();
            code.WrapBracesAround(() =>
            {
                foreach (var value in values) code.AppendLine($"{value.Key} = {value.Value:D},");
            });
            code.AppendLine();
        }

        /// <summary>
        /// Writes the code declaring a method
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">The name of the method</param>
        /// <param name="content">The action to write the content of the method</param>
        /// <param name="parameters">If specified, declares the method with parameters</param>
        /// <param name="returns">If specified, declares the method with the specified return type, otherwise is void</param>
        /// <param name="modifiers"></param>
        public static void DeclareMethod(
            this StringBuilder code,
            string name,
            Action? content = null,
            IReadOnlyCollection<string>? parameters = null,
            string? returns = null,
            string? modifiers = null)
        {
            code.Append("public");
            if (modifiers != null) code.Append($" {modifiers}");
            code.Append($" {returns ?? "void"} {name}");
            code.DeclareParameters(parameters);
            if (content != null) code.WrapBracesAround(content);
            else code.Append(";");
            code.AppendLine();
            code.AppendLine();
        }

        /// <summary>
        /// Writes the code declaring a list of parameters for a method
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="parameters">The parameters to be written. The key is the name, and the value is the type</param>
        public static void DeclareParameters(this StringBuilder code, IReadOnlyCollection<string>? parameters)
        {
            code.WrapParenthesisAround(() =>
            {
                if (parameters == null) return;

                code.Append(string.Join(", ", parameters));
            });
        }

        /// <summary>
        /// Writes the code declaring a property
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="name">The name of the property</param>
        /// <param name="type">The type of the property</param>
        /// <param name="defaultAssignment">Optional default assignment of the property</param>
        public static void DeclareProperty(this StringBuilder code, string name, string type, string defaultAssignment = "default") => code.AppendLine(@$"public {type} {name} {{ get; set; }} = {defaultAssignment};");

        /// <summary>
        /// Writes the code to declare a variable, returning the generated variable name.
        /// </summary>
        /// <param name="code">The code string builder</param>
        /// <param name="context"></param>
        /// <param name="type">The type of the property, if needed to be explicit</param>
        /// <param name="assignment">The assignment to that property, if needed.</param>
        public static string DeclareVariable(this StringBuilder code, ConstructionContext context, string type = "var", Action? assignment = null)
        {
            var variableName = context.NewVarName();

            code.AsStatement(() =>
            {
                code.Append($@"{type} {variableName}");
                if (assignment == null) return;
                
                code.Append(" = ");
                assignment();
            });

            return variableName;
        }

        public static void DeclareIfStatement(this StringBuilder code, Action condition, Action then, Action? otherwise = null)
        {
            code.Append("if ");
            code.WrapParenthesisAround(condition);
            code.WrapBracesAround(then);

            if (otherwise != null)
            {
                code.Append(" else ");
                code.WrapBracesAround(otherwise);
            }

            code.AppendLine();
        }
    }
}
