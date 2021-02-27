// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using DFC.App.JobProfile.Cacheing;
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Providers;
using DFC.App.JobProfile.EventProcessing;
using DFC.App.JobProfile.EventProcessing.Models;
using DFC.App.JobProfile.HostedServices;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.Webhooks;
using DFC.Compui.Cosmos;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DFC.App.JobProfile
{
    public class Startup
    {
        public const string JobProfileStoreSettings = "Configuration:DocumentStore:JobProfile";
        public const string CurrentOpportunitiesStoreSettings = "Configuration:DocumentStore:CurrentOpportunities";
        public const string StaticContentStoreSettings = "Configuration:DocumentStore:StaticContent";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _envvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _envvironment = env;
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // TODO: check this!!! CorrelationId breaking changes...
            /*
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "DssCorrelationId",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false,
            });
            */

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(typeof(Startup).Assembly);

            // TODO: check this!!! breaking changes...
            // services.AddCorrelationId()
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            AddApplicationSpecificDependencyInjectionConfiguration(services);
        }

        private void AddApplicationSpecificDependencyInjectionConfiguration(IServiceCollection services)
        {
            /*
            var eventGridSubscriptionModel = configuration.GetSection(nameof(EventGridSubscriptionModel)).Get<EventGridSubscriptionModel>() ?? new EventGridSubscriptionModel();
            eventGridSubscriptionModel.Name = configuration.GetValue("Configuration:ApplicationName", typeof(Startup).Namespace!.Replace(".", "-", System.StringComparison.OrdinalIgnoreCase));
            services.AddSingleton(eventGridSubscriptionModel);
            */

            //var jpStorageProperties = _configuration.GetSection(JobProfileStoreSettings).Get<JobProfileStorageProperties>();
            //var scStorageProperties = _configuration.GetSection(StaticContentStoreSettings).Get<StaticContentStorageProperties>();
            //var documentClient = new DocumentClient(new Uri(jpStorageProperties.EndpointLocation), jpStorageProperties.AccessKey);

            //services.AddSingleton(jpStorageProperties);
            //services.AddSingleton<IDocumentClient>(documentClient);

            //services.AddSingleton(scStorageProperties);
            //services.AddSingleton<IStaticCosmosRepository<StaticContentItemModel>, StaticCosmosRepository<StaticContentItemModel>>();

            services.AddScoped<IProvideJobProfiles, JobProfileProvider>();
            services.AddScoped<IProvideCurrentOpportunities, CurrentOpportunitiesProvider>();
            //services.AddScoped<ISharedContentService, SharedContentService>();
            //services.AddTransient<CorrelationIdDelegatingHandler>();

            // services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"])
            services.AddSingleton(_configuration.GetSection(nameof(FeedbackLinks)).Get<FeedbackLinks>() ?? new FeedbackLinks());

            //TODO: what's this and do we need it??
            //services.AddSingleton(_configuration.GetSection(nameof(OverviewDetails)).Get<OverviewDetails>() ?? new OverviewDetails());

            services.AddApplicationInsightsTelemetry();

            var jpStorageProperties = _configuration.GetSection(JobProfileStoreSettings).Get<CosmosDbConnection>();
            var coStorageProperties = _configuration.GetSection(CurrentOpportunitiesStoreSettings).Get<CosmosDbConnection>();
            var scStorageProperties = _configuration.GetSection(StaticContentStoreSettings).Get<CosmosDbConnection>();
            services.AddContentPageServices<JobProfileCached>(jpStorageProperties, _envvironment.IsDevelopment());
            services.AddContentPageServices<CurrentOpportunities>(coStorageProperties, _envvironment.IsDevelopment());
            services.AddContentPageServices<ContentApiStaticElement>(scStorageProperties, _envvironment.IsDevelopment());

            // remote contract local service implementation
            //services.AddSingleton<IContentCacheService, CacheContentService.ContentCacheService>();

            services.AddTransient<ILoadJobProfileContent, JobProfileCacheLoader<JobProfileCached>>();
            services.AddTransient<ILoadStaticContent, StaticContentLoader>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddTransient<IProvideGraphContent, GraphContentProvider>();
            services.AddTransient<IProcessGraphCuries, GraphCurieProcessor>();

            services.AddTransient<IWebhooksService, WebhooksService>();

            services.AddTransient<IEventMessageService<JobProfileCached>, EventMessageService<JobProfileCached>>();
            services.AddTransient<IEventMessageService<ContentApiStaticElement>, EventMessageService<ContentApiStaticElement>>();
            services.AddTransient<IEventGridService<JobProfileCached>, EventGridService<JobProfileCached>>();
            services.AddTransient<IEventGridClientService, EventGridClientService>();

            services.AddSingleton(_configuration.GetSection(nameof(ContentApiOptions)).Get<ContentApiOptions>());
            services.AddSingleton(_configuration.GetSection(nameof(EventGridSubscriptionClientOptions)).Get<EventGridSubscriptionClientOptions>() ?? new EventGridSubscriptionClientOptions());
            services.AddSingleton(_configuration.GetSection(nameof(EventGridPublishClientOptions)).Get<EventGridPublishClientOptions>() ?? new EventGridPublishClientOptions());
            services.AddHostedServiceTelemetryWrapper();

            services.AddSubscriptionBackgroundService(_configuration);
            services.AddHostedService<StaticContentBackgroundLoader>();
            services.AddHostedService<JobProfileBackgroundLoader>();

            // TODO: remove me!
            // const string AppSettingsPolicies = "Policies"
            // var policyOptions = _configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>()
            var policyRegistry = services.AddPolicyRegistry();

            services.AddApiServices(_configuration, policyRegistry);
        }
    }
}