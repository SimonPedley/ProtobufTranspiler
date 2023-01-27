using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using ProtobufTranspiler.Generator.Exceptions;
using YamlDotNet.Serialization;

namespace ProtobufTranspiler.Generator.Transpilation.Loading;

public interface IProjectLoader
{
    Task<ProjectContext> LoadProjectContext(string projectFilePath, CancellationToken cancellationToken);
}

public class ProjectLoader : IProjectLoader
{
    public const string ProjectFileExtension = @"csproj";
    public const string ConfigFileName = "proto-transpiler.yaml";

    private readonly Deserializer _deserialiser;

    public ProjectLoader() => _deserialiser = new Deserializer();

    public async Task<ProjectContext> LoadProjectContext(string projectFilePath, CancellationToken cancellationToken)
    {
        if (Path.GetExtension(projectFilePath) != $@".{ProjectFileExtension}")
            throw new TranspilerProjectAnalysisException($@"Project path '{projectFilePath}' is invalid. '*.{ProjectFileExtension}' file expected");
        if (!File.Exists(projectFilePath)) throw new TranspilerProjectAnalysisException($@"Project path '{projectFilePath}' is invalid. File could not be found");

        var compilationTask = GetCompilation(projectFilePath, cancellationToken);
        var transpilerConfigTask = LoadProtoTranspilerConfig(projectFilePath);
        return new ProjectContext(
            Path.GetDirectoryName(projectFilePath) ?? throw new TranspilerProjectAnalysisException("Could not get project directory"),
            await compilationTask,
            await transpilerConfigTask);
    }

    private static async Task<Compilation> GetCompilation(string projectFilePath, CancellationToken cancellationToken)
    {
        using var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(projectFilePath, cancellationToken: cancellationToken);
        return await project.GetCompilationAsync(cancellationToken) ?? throw new TranspilerProjectAnalysisException($"Failed to compile project {projectFilePath}");
    }

    private async Task<TranspilerConfig> LoadProtoTranspilerConfig(string projectFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath)!;
        var configFilePath = Path.Combine(projectDirectory, ConfigFileName);

        if (!File.Exists(configFilePath)) return new TranspilerConfig();

        try
        {
            await using var fileStream = File.Open(configFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(fileStream, Encoding.UTF8);

            return _deserialiser.Deserialize<TranspilerConfig>(streamReader);
        }
        catch (Exception exception)
        {
            throw new TranspilerProjectAnalysisException(@"Config YAML could not be parsed", exception);
        }
    }
}
