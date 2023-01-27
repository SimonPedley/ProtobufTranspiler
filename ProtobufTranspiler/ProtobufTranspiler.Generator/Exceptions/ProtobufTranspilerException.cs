using System.Runtime.Serialization;

namespace ProtobufTranspiler.Generator.Exceptions;

public class ProtobufTranspilerException : Exception
{
    public ProtobufTranspilerException()
    {
    }

    protected ProtobufTranspilerException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ProtobufTranspilerException(string? message) : base(message)
    {
    }

    public ProtobufTranspilerException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
