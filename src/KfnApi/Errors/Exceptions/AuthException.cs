using System.Runtime.Serialization;

namespace KfnApi.Errors.Exceptions;

[Serializable]
public class AuthException : Exception
{
    public AuthException(string message) : base(message) { }

    protected AuthException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
