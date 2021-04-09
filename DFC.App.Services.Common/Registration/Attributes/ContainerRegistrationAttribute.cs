using DFC.App.Services.Common.Helpers;
using System;

namespace DFC.App.Services.Common.Registration.Attributes
{
    /// <summary>
    /// The container targeting registration attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public abstract class ContainerRegistrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="scope">The life container scope.</param>
        protected ContainerRegistrationAttribute(Type contractType, Type implementationType, TypeOfRegistrationScope scope)
        {
            (!contractType.IsAssignableFrom(implementationType))
                .AsGuard<ArgumentException>(implementationType.Name);

            ContractType = contractType;
            ImplementationType = implementationType;
            Scope = scope;
        }

        /// <summary>
        /// Gets the contract type.
        /// </summary>
        public Type ContractType { get; }

        /// <summary>
        /// Gets the implementation type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// Gets the (container lifetime) scope.
        /// </summary>
        public TypeOfRegistrationScope Scope { get; }
    }
}
