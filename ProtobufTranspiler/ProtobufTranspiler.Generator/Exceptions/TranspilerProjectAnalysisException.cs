using System.Runtime.Serialization;

namespace ProtobufTranspiler.Generator.Exceptions;

public class TranspilerProjectAnalysisException : ProtobufTranspilerException
{
    public TranspilerProjectAnalysisException()
    {
    }

    protected TranspilerProjectAnalysisException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TranspilerProjectAnalysisException(string? message) : base(message)
    {
    }

    public TranspilerProjectAnalysisException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
