using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.Data.Models;
using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DFC.App.JobProfile
{
    public class Startup
    {
        private const string JobProfileStoreSettings = "Configuration:DocumentStore:JobProfile";
        private const string CurrentOpportunitiesStoreSettings = "Configuration:DocumentStore:CurrentOpportunities";
        private const string SegmentOpportunitiesStoreSettings = "Configuration:DocumentStore:OldCurrentOpportunities";
        private const string StaticContentStoreSettings = "Configuration:DocumentStore:StaticContent";

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // add the site map route
                endpoints.MapControllerRoute(
                    name: "Sitemap",
                    pattern: "Sitemap.xml",
                    defaults: new { controller = "Sitemap", action = "Sitemap" });

                // add the robots.txt route
                endpoints.MapControllerRoute(
                    name: "Robots",
                    pattern: "Robots.txt",
                    defaults: new { controller = "Robot", action = "Robot" });

                // add the default route as health/ping
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Health}/{action=Ping}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var policyRegistry = services.AddPolicyRegistry();
            var isDev = _environment.IsDevelopment();
            var jpStorageProperties = _configuration.GetSection(JobProfileStoreSettings).Get<CosmosDbConnection>();
            var coStorageProperties = _configuration.GetSection(CurrentOpportunitiesStoreSettings).Get<CosmosDbConnection>();
            var ocoStorageProperties = _configuration.GetSection(SegmentOpportunitiesStoreSettings).Get<CosmosDbConnection>();
            var scStorageProperties = _configuration.GetSection(StaticContentStoreSettings).Get<CosmosDbConnection>();

            var registrar = ServiceRegistrar.Create(_configuration);
            registrar.RegisterWith(services);

            services
                .AddApplicationInsightsTelemetry()
                .AddAutoMapper(typeof(Startup).Assembly)
                .AddHostedServiceTelemetryWrapper()
                .AddSubscriptionBackgroundService(_configuration)
                .AddApiServices(_configuration, policyRegistry)
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .AddContentPageServices<JobProfileCached>(jpStorageProperties, isDev)
                .AddContentPageServices<CurrentOpportunities>(coStorageProperties, isDev)
                .AddContentPageServices<SegmentCurrentOpportunity>(ocoStorageProperties, isDev)
                .AddContentPageServices<StaticItemCached>(scStorageProperties, isDev)
                .AddControllersWithViews();
        }
    }
}