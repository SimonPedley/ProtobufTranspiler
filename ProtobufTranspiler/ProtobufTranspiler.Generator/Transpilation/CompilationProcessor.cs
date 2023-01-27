using ProtobufTranspiler.Generator.Transpilation.Loading;

namespace ProtobufTranspiler.Generator.Transpilation;

public interface ICompilationTranspiler
{
    Task Process(string projectFilePath, CancellationToken cancellationToken);
}

public class CompilationTranspiler : ICompilationTranspiler
{
    private readonly IProjectLoader _projectLoader;

    public CompilationTranspiler(IProjectLoader projectLoader)
    {
        _projectLoader = projectLoader;
    }

    public async Task Process(string projectFilePath, CancellationToken cancellationToken)
    {
        var projectContext = await _projectLoader.LoadProjectContext(projectFilePath, cancellationToken);
        
        Console.WriteLine($@"Project Root: {projectContext.ProjectRoot}");
    }
}
