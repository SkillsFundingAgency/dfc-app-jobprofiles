using System;

namespace DFC.App.Services.Common.Registration.Attributes
{
    /// <summary>
    /// The external targeting registration attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ExternalRegistrationAttribute :
        ContainerRegistrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="scope">The life container scope.</param>
        public ExternalRegistrationAttribute(Type contractType, Type implementationType, TypeOfRegistrationScope scope)
            : base(contractType, implementationType, scope)
        {
        }
    }
}
