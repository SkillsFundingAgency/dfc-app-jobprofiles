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
    public class ConflictingResourceException : Exception
    {
        public const string ExceptionMessage = "Resource already exists";

        public ConflictingResourceException()
            : this(ExceptionMessage)
        {
        }

        public ConflictingResourceException(string message)
            : base(message)
        {
        }

        public ConflictingResourceException(string message, Exception innerException)
            : base(ExceptionMessage, innerException)
        {
        }

        protected ConflictingResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}