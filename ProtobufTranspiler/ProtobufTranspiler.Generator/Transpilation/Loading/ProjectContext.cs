using Microsoft.CodeAnalysis;

namespace ProtobufTranspiler.Generator.Transpilation.Loading;

public record ProjectContext(
    string ProjectRoot,
    Compilation Compilation,
    TranspilerConfig Config);
