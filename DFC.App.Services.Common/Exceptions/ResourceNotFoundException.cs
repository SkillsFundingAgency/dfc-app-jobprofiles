using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Faults
{
    /// <summary>
    /// Conflicting resource exception.
    /// Constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ResourceNotFoundException : Exception
    {
        public const string ExceptionMessage = "Resource not found";

        public ResourceNotFoundException()
            : this(ExceptionMessage)
        {
        }

        public ResourceNotFoundException(string message)
            : base(message)
        {
        }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}