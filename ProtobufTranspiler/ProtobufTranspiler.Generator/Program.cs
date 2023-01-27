using ProtobufTranspiler.Generator.Transpilation;
using ProtobufTranspiler.Generator.Transpilation.Loading;

ICompilationTranspiler transpiler = new CompilationTranspiler(new ProjectLoader());

await transpiler.Process(@"C:\repos\ProtobufTranspiler\ProtobufTranspiler\ProtobufTranspiler.Generator.Example\ProtobufTranspiler.Generator.Example.csproj", CancellationToken.None);
