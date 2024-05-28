using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DFC.App.JobProfile.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public CustomWebApplicationFactory()
        {
            this.FakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
        }

        internal ISharedContentRedisInterface FakeSharedContentRedisInterface { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder?.ConfigureServices(services =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                services.AddSingleton<IConfiguration>(configuration);
            });

            builder.ConfigureTestServices(services =>
            {
                var hostedServices = services.Where(descriptor =>
                    descriptor.ServiceType == typeof(ISharedContentRedisInterface))
                .ToList();

                foreach (var service in hostedServices)
                {
                    services.Remove(service);
                }

                services.AddTransient(sp => FakeSharedContentRedisInterface);
            });
        }
    }
}
