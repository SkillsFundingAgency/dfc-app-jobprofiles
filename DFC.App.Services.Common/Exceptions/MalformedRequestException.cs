using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Faults
{
    /// <summary>
    /// Malformed request exception.
    /// Constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MalformedRequestException : Exception
    {
        public MalformedRequestException()
            : this(string.Empty)
        {
        }

        public MalformedRequestException(string message)
            : base(message)
        {
        }

        public MalformedRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MalformedRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}