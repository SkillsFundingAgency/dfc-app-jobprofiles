using DFC.App.JobProfile.CacheContentService;
using DFC.App.JobProfile.ClientHandlers;
using DFC.App.JobProfile.Contracts;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ClientOptions;
using DFC.App.JobProfile.EventProcessorService;
using DFC.App.JobProfile.HostedServices;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.App.JobProfile.Repository.CosmosDb;
using DFC.Compui.Cosmos;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Extensions;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DFC.App.JobProfile
{
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfile";
        public const string StaticCosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:StaticContent";
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
            [
                Header = "DssCorrelationId",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false,
            ])
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
            var eventGridSubscriptionModel = configuration.GetSection(nameof(EventGridSubscriptionModel)).Get<EventGridSubscriptionModel>() ?? new EventGridSubscriptionModel()
            eventGridSubscriptionModel.Name = configuration.GetValue("Configuration:ApplicationName", typeof(Startup).Namespace!.Replace(".", "-", System.StringComparison.OrdinalIgnoreCase))
            services.AddSingleton(eventGridSubscriptionModel)
            */

            var cosmosDbConnection = _configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            var staticContentDbConnection = _configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<StaticCosmosDbConnection>();
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey);

            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfileModel>, CosmosRepository<JobProfileModel>>();

            services.AddSingleton(staticContentDbConnection);
            services.AddSingleton<IStaticCosmosRepository<StaticContentItemModel>, StaticCosmosRepository<StaticContentItemModel>>();

            services.AddScoped<IJobProfileService, JobProfileService>();
            services.AddScoped<ISharedContentService, SharedContentService>();
            services.AddTransient<CorrelationIdDelegatingHandler>();

            // services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"])
            services.AddSingleton(_configuration.GetSection(nameof(FeedbackLinks)).Get<FeedbackLinks>() ?? new FeedbackLinks());
            services.AddSingleton(_configuration.GetSection(nameof(OverviewDetails)).Get<OverviewDetails>() ?? new OverviewDetails());

            services.AddApplicationInsightsTelemetry();
            var cosmosDbConnectionContentPages = _configuration.GetSection(CosmosDbConfigAppSettings).Get<Compui.Cosmos.Contracts.CosmosDbConnection>();
            var cosmosDbConnectionStaticPages = _configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<Compui.Cosmos.Contracts.CosmosDbConnection>();
            services.AddContentPageServices<JobProfileContentPageModel>(cosmosDbConnectionContentPages, _envvironment.IsDevelopment());
            services.AddContentPageServices<StaticContentItemModel>(cosmosDbConnectionStaticPages, _envvironment.IsDevelopment());

            // remote contract local service implementation
            services.AddSingleton<IContentCacheService, ContentCacheService>();

            services.AddTransient<IEventMessageService<JobProfileContentPageModel>, EventMessageService<JobProfileContentPageModel>>();
            services.AddTransient<IEventMessageService<StaticContentItemModel>, EventMessageService<StaticContentItemModel>>();
            services.AddTransient<ILoadJobProfileContent, JobProfileCacheLoader>();
            services.AddTransient<ILoadStaticContent, StaticContentLoader>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<ICmsApiService, CmsApiService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IEventGridService, EventGridService>();
            services.AddTransient<IEventGridClientService, EventGridClientService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddSingleton(_configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
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