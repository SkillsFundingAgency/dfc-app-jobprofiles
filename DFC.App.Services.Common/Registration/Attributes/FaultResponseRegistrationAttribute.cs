using DFC.App.Services.Common.Helpers;
using System;
using System.Net;

namespace DFC.App.Services.Common.Registration.Attributes
{
    /// <summary>
    /// The fault response registration attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class FaultResponseRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FaultResponseRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="exceptionType">The exception type.</param>
        /// <param name="responseCode">The response code.</param>
        public FaultResponseRegistrationAttribute(Type exceptionType, HttpStatusCode responseCode)
        {
            (!typeof(Exception).IsAssignableFrom(exceptionType))
                .AsGuard<ArgumentException>(exceptionType.Name);

            ExceptionType = exceptionType;
            ResponseCode = responseCode;
        }

        /// <summary>
        /// Gets the exception type.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Gets the response code.
        /// </summary>
        public HttpStatusCode ResponseCode { get; }
    }
}
