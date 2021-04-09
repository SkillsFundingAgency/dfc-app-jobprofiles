using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Faults
{
    /// <summary>
    /// Fallback action exception.
    /// Constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class FallbackActionException : Exception
    {
        public FallbackActionException()
        {
        }

        public FallbackActionException(string message)
            : base(message)
        {
        }

        public FallbackActionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FallbackActionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}