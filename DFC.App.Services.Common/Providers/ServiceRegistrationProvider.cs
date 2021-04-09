using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DFC.App.Services.Common.Providers
{
    public sealed class ServiceRegistrationProvider :
        IProvideRegistrationServices
    {
        private bool isInitialised = false;

        [ExcludeFromCodeCoverage]
        public ServiceRegistrationProvider(
            IProvideRegistrationDetails details,
            IConfiguration configuration)
        {
            It.IsNull(details)
                .AsGuard<ArgumentNullException>(nameof(details));

            ActionMap = new RegistrationMap
            {
                [TypeOfRegistrationScope.Scoped] = AddScoped,
                [TypeOfRegistrationScope.Transient] = AddTransient,
                [TypeOfRegistrationScope.Singleton] = AddSingleton,
            };

            Details = details;
            Configuration = configuration;
        }

        internal IProvideRegistrationDetails Details { get; }

        internal IConfiguration Configuration { get; }

        internal RegistrationMap ActionMap { get; }

        internal ICollection<ConfigurationRegistrationAttribute> Configurations { get; }
            = Collection.Empty<ConfigurationRegistrationAttribute>();

        internal ICollection<ContainerRegistrationAttribute> Registrations { get; }
            = Collection.Empty<ContainerRegistrationAttribute>();

        public void RegisterWith(IServiceCollection containerCollection)
        {
            Initialise();

            Configurations.ForEach(registration => RegisterConfiguration(containerCollection, registration));
            Registrations.ForEach(registration => RegisterService(containerCollection, registration));
        }

        internal void Initialise()
        {
            if (isInitialised)
            {
                return;
            }

            isInitialised = true;
            var cache = Details.Load();
            cache.ForEach(LoadRegistrations);
        }

        internal void LoadRegistrations(Assembly registrant)
        {
            It.IsNull(registrant)
                .AsGuard<ArgumentNullException>(nameof(registrant));

            Details.GetAttributeListFor<ConfigurationRegistrationAttribute>(registrant)
                .ForEach(Configurations.Add);
            Details.GetAttributeListFor<ExternalRegistrationAttribute>(registrant)
                .ForEach(Registrations.Add);
            Details.GetAttributeListFor<InternalRegistrationAttribute>(registrant)
                .ForEach(Registrations.Add);
        }

        internal void RegisterConfiguration(IServiceCollection containerCollection, ConfigurationRegistrationAttribute registration)
        {
            It.IsNull(Configuration)
                .AsGuard<ArgumentNullException>(nameof(Configuration));

            var instance = Activator.CreateInstance(registration.ImplementationType);
            ConfigureItem(registration, instance);
            containerCollection.AddSingleton(registration.ContractType, instance);
        }

        internal void ConfigureItem(ConfigurationRegistrationAttribute registration, object item)
        {
            Configuration.GetSection(registration.Section).Bind(item);
        }

        internal void RegisterService(IServiceCollection containerCollection, ContainerRegistrationAttribute registration)
        {
            ActionMap[registration.Scope].Invoke(containerCollection, registration);
        }

        internal void AddScoped(IServiceCollection containerCollection, ContainerRegistrationAttribute registration) =>
            containerCollection.AddScoped(registration.ContractType, registration.ImplementationType);

        internal void AddTransient(IServiceCollection containerCollection, ContainerRegistrationAttribute registration) =>
            containerCollection.AddTransient(registration.ContractType, registration.ImplementationType);

        internal void AddSingleton(IServiceCollection containerCollection, ContainerRegistrationAttribute registration) =>
            containerCollection.AddSingleton(registration.ContractType, registration.ImplementationType);
    }
}
