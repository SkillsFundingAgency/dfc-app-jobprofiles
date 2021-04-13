using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Faults
{
    /// <summary>
    /// No content exception
    /// Constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class NoContentException : Exception
    {
        public NoContentException()
            : base(GetMessage())
        {
        }

        public NoContentException(string message)
            : base(message)
        {
        }

        public NoContentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoContentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public static string GetMessage(string parentResourceName = null) =>
            parentResourceName != null
                ? $"'{parentResourceName}' does not exist"
                : "Resource does not exist";
    }
}