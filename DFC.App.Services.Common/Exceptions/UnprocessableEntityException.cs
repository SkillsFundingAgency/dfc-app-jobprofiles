using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Faults
{
    /// <summary>
    /// Unprocessable entity exception
    /// Constructors and decorators are here to satisfy the static analysis tool
    /// as a consequence, excluded from coverage as they can't be tested properly.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class UnprocessableEntityException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException"/> class.
        /// </summary>
        public UnprocessableEntityException()
            : base(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException"/> class.
        /// </summary>
        /// <param name="message">message.</param>
        public UnprocessableEntityException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException"/> class.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="innerException">inner exception.</param>
        public UnprocessableEntityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnprocessableEntityException"/> class.
        /// </summary>
        /// <param name="info">info.</param>
        /// <param name="context">context.</param>
        protected UnprocessableEntityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}