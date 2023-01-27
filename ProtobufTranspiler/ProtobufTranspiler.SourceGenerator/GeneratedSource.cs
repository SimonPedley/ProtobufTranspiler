namespace ProtobufTranspiler.SourceGenerator;

public class GeneratedSource
{
    public string FileName { get; }

    public string Code { get; }

    public GeneratedSource(string fileName, string code)
    {
        FileName = fileName;
        Code = code;
    }
}
