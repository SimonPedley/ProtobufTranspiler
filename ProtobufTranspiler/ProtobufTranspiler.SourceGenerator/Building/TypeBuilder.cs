namespace ProtobufTranspiler.SourceGenerator.Building
{
    public static class TypeBuilder
    {
        /// <summary>
        /// Builds all the target message types
        /// </summary>
        /// <param name="context"></param>
        public static IEnumerable<GeneratedSource> Build(ConstructionContext context) =>
            context.TypeImplementations.Values
                .SelectMany(implementation => implementation.GenerateTypes(context))
                .Select(source => new GeneratedSource($@"{source.FileName}.type.cs", source.Code));
    }
}
