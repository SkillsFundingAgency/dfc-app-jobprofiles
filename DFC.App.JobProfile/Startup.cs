using AutoMapper;
using CorrelationId;
using DFC.App.JobProfile.CacheContentService;
using DFC.App.JobProfile.ClientHandlers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.ClientOptions;
using DFC.App.JobProfile.EventProcessorService;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.HostedServices;
using DFC.App.JobProfile.HttpClientPolicies;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.App.JobProfile.Repository.CosmosDb;
using DFC.Compui.Cosmos;
using DFC.Compui.Telemetry;
using DFC.Logger.AppInsights.Extensions;
using dfc_content_pkg_netcore.ApiProcessorService;
using dfc_content_pkg_netcore.CmsApiProcessorService;
using dfc_content_pkg_netcore.contracts;
using dfc_content_pkg_netcore.models;
using dfc_content_pkg_netcore.models.clientOptions;
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
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfile";
        public const string StaticCosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:StaticContent";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "DssCorrelationId",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false,
            });

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

            //mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddCorrelationId();
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
            var eventGridSubscriptionModel = configuration.GetSection(nameof(EventGridSubscriptionModel)).Get<EventGridSubscriptionModel>() ?? new EventGridSubscriptionModel();
            eventGridSubscriptionModel.Name = configuration.GetValue("Configuration:ApplicationName", typeof(Startup).Namespace!.Replace(".", "-", System.StringComparison.OrdinalIgnoreCase));
            services.AddSingleton(eventGridSubscriptionModel);
            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            var staticContentDbConnection = configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<StaticCosmosDbConnection>();
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey);

            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfileModel>, CosmosRepository<JobProfileModel>>();

            services.AddSingleton(staticContentDbConnection);
            services.AddSingleton<IStaticCosmosRepository<StaticContentItemModel>, StaticCosmosRepository<StaticContentItemModel>>();

            services.AddScoped<IJobProfileService, JobProfileService>();
            services.AddScoped<ISharedContentService, SharedContentService>();
            services.AddTransient<CorrelationIdDelegatingHandler>();
            services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);
            services.AddSingleton(configuration.GetSection(nameof(FeedbackLinks)).Get<FeedbackLinks>() ?? new FeedbackLinks());

            services.AddApplicationInsightsTelemetry();
            var cosmosDbConnectionContentPages = configuration.GetSection(CosmosDbConfigAppSettings).Get<Compui.Cosmos.Contracts.CosmosDbConnection>();
            var cosmosDbConnectionStaticPages = configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<Compui.Cosmos.Contracts.CosmosDbConnection>();
            services.AddContentPageServices<ContentPageModel>(cosmosDbConnectionContentPages, env.IsDevelopment());
            services.AddContentPageServices<StaticContentItemModel>(cosmosDbConnectionStaticPages, env.IsDevelopment());
            services.AddSingleton<IContentCacheService>(new ContentCacheService());
            services.AddTransient<IEventMessageService<ContentPageModel>, EventMessageService<ContentPageModel>>();
            services.AddTransient<IEventMessageService<StaticContentItemModel>, EventMessageService<StaticContentItemModel>>();
            services.AddTransient<ICacheReloadService, CacheReloadService>();
            services.AddTransient<IStaticContentReloadService, StaticContentReloadService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddSingleton(configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            services.AddSingleton(configuration.GetSection(nameof(EventGridSubscriptionClientOptions)).Get<EventGridSubscriptionClientOptions>() ?? new EventGridSubscriptionClientOptions());
            services.AddHostedServiceTelemetryWrapper();
            services.AddHostedService<StaticContentReloadBackgroundService>();
            services.AddHostedService<CacheReloadBackgroundService>();
            services.AddHostedService<CreateSubscriptionBackgroundService>();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>();
            var policyRegistry = services.AddPolicyRegistry();

            services
               .AddPolicies(policyRegistry, nameof(CmsApiClientOptions), policyOptions)
               .AddHttpClient<ICmsApiService, CmsApiService, CmsApiClientOptions>(configuration, nameof(CmsApiClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
               .AddPolicies(policyRegistry, nameof(EventGridSubscriptionClientOptions), policyOptions)
               .AddHttpClient<IEventGridSubscriptionService, EventGridSubscriptionService, EventGridSubscriptionClientOptions>(configuration, nameof(EventGridSubscriptionClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));
        }
    }
}