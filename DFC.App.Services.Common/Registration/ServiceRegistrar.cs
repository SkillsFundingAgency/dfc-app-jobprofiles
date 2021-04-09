using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Providers;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Services.Common.Registration
{
    /// <summary>
    /// The service registration factory.
    /// </summary>
    public static class ServiceRegistrar
    {
        /// <summary>
        /// Create...
        /// Because this is a bootrapping process,
        /// this class has to be static and we have to new things up.
        /// </summary>
        /// <param name="configuration">The configuration binder.</param>
        /// <returns>A service registrar.</returns>
        [ExcludeFromCodeCoverage]
        public static IProvideRegistrationServices Create(IConfiguration configuration)
        {
            var assets = new AssetProvider();
            var converter = new JsonTypeConverter();
            var details = new RegistrationDetailProvider(assets, converter);

            return new ServiceRegistrationProvider(details, configuration);
        }
    }
}
