using DFC.App.Services.Common.Helpers;
using System;

namespace DFC.App.Services.Common.Registration.Attributes
{
    /// <summary>
    /// The configuration registration attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ConfigurationRegistrationAttribute :
        ContainerRegistrationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="configurationSection">The configuration section.</param>
        public ConfigurationRegistrationAttribute(Type contractType, Type implementationType, string configurationSection)
            : base(contractType, implementationType, TypeOfRegistrationScope.Singleton)
        {
            Section = configurationSection;

            // The implementation should support service registration, rather than the contract type.
            // But we check both.
            (!SupportConfigurationRegistration(implementationType) && !SupportConfigurationRegistration(contractType))
                .AsGuard<ArgumentException>(implementationType.Name);
        }

        /// <summary>
        /// Gets the response code.
        /// </summary>
        public string Section { get; }

        /// <summary>
        /// Support configuration registration.
        /// </summary>
        /// <param name="givenType">(The) given type.</param>
        /// <returns>True if supported.</returns>
        public bool SupportConfigurationRegistration(Type givenType) => typeof(IRequireConfigurationRegistration).IsAssignableFrom(givenType);
    }
}
