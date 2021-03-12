// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
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
        public const string JobProfileStoreSettings = "Configuration:DocumentStore:JobProfile";
        public const string CurrentOpportunitiesStoreSettings = "Configuration:DocumentStore:CurrentOpportunities";
        public const string SegmentOpportunitiesStoreSettings = "Configuration:DocumentStore:OldCurrentOpportunities";
        public const string StaticContentStoreSettings = "Configuration:DocumentStore:StaticContent";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _environment = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // check this!!! breaking changes...
            //app.UseCorrelationId(new CorrelationIdOptions
            //{
            //    Header = "DssCorrelationId",
            //    UseGuidForCorrelationId = true,
            //    UpdateTraceIdentifier = false,
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // see https://aka.ms/aspnetcore-hsts.
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
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                })
                .AddContentPageServices<JobProfileCached>(jpStorageProperties, isDev)
                .AddContentPageServices<CurrentOpportunities>(coStorageProperties, isDev)
                .AddContentPageServices<SegmentCurrentOpportunity>(ocoStorageProperties, isDev)
                .AddContentPageServices<StaticItemCached>(scStorageProperties, isDev)
                .AddControllersWithViews();

            // check this!!! breaking changes...
            // services.AddCorrelationId()

            // what are these and do we need them??
            //services.AddScoped<ISharedContentService, SharedContentService>();
            //services.AddTransient<CorrelationIdDelegatingHandler>();
            //services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"])
            //services.AddSingleton(_configuration.GetSection(nameof(OverviewDetails)).Get<OverviewDetails>() ?? new OverviewDetails());
            //services.AddSingleton<IContentCacheService, CacheContentService.ContentCacheService>();
        }
    }
}