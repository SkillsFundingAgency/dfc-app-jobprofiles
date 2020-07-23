using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.JobProfile.Exceptions
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class InvalidProfileException : Exception
    {
        public InvalidProfileException() : base()
        {
        }

        public InvalidProfileException(string message) : base(message)
        {
        }

        public InvalidProfileException(string message, Exception exception) : base(message, exception)
        {
        }

        protected InvalidProfileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}