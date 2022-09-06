using AutoMapper;
using CorrelationId;
using CorrelationId.DependencyInjection;
using DFC.App.JobProfile.ClientHandlers;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.HostedServices;
using DFC.App.JobProfile.HttpClientPolicies;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.App.JobProfile.ProfileService.SegmentServices;
using DFC.App.JobProfile.Repository.CosmosDb;
using DFC.App.JobProfile.Services;
using DFC.Compui.Cosmos;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService;
using DFC.Logger.AppInsights.Extensions;
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
        public const string CosmosDbSharedContentConfigAppSettings = "Configuration:CosmosDbConnections:SharedContent";
        public const string ConfigAppSettings = "Configuration";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        //private readonly IConfiguration configuration;
        //private readonly IWebHostEnvironment env;

        //public Startup(IConfiguration configuration, IWebHostEnvironment env)
        //{
        //    this.configuration = configuration;
        //    this.env = env;
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
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

            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultCorrelationId(options =>
            {
                options.RequestHeader = "DssCorrelationId";
                options.UpdateTraceIdentifier = false;
            });

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
            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();

            services.AddSingleton(cosmosDbConnection);

            var retryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey, new ConnectionPolicy { RetryOptions = retryOptions });
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfileModel>, CosmosRepository<JobProfileModel>>();
            services.AddSingleton<IRedirectionSecurityService, RedirectionSecurityService>();

            var configValues = configuration.GetSection(ConfigAppSettings).Get<ConfigValues>();
            services.AddSingleton(configValues);

            services.AddScoped<IJobProfileService, JobProfileService>();
            services.AddScoped<ISegmentService, SegmentService>();
            services.AddTransient<CorrelationIdDelegatingHandler>();
            services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddSingleton(configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            services.AddTransient<ISharedContentReloadService, SharedContentReloadService>();
            services.AddTransient<ICmsApiService, CmsApiService>();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddTransient<IApiCacheService, ApiCacheService>();
            services.AddTransient<IContentTypeMappingService, ContentTypeMappingService>();

            services.AddHostedServiceTelemetryWrapper();
            services.AddHostedService<SharedContentReloadBackgroundService>();

            services.AddSingleton(configuration.GetSection(nameof(CareerPathSegmentClientOptions)).Get<CareerPathSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(CurrentOpportunitiesSegmentClientOptions)).Get<CurrentOpportunitiesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(FeedbackLinks)).Get<FeedbackLinks>() ?? new FeedbackLinks());
            services.AddSingleton(configuration.GetSection(nameof(HowToBecomeSegmentClientOptions)).Get<HowToBecomeSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(OverviewBannerSegmentClientOptions)).Get<OverviewBannerSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(RelatedCareersSegmentClientOptions)).Get<RelatedCareersSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatItTakesSegmentClientOptions)).Get<WhatItTakesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatYouWillDoSegmentClientOptions)).Get<WhatYouWillDoSegmentClientOptions>());

            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            var cosmosDbConnectionSharedContent = configuration.GetSection(CosmosDbSharedContentConfigAppSettings).Get<Compui.Cosmos.Contracts.CosmosDbConnection>();
            services.AddDocumentServices<SharedContentItemModel>(cosmosDbConnectionSharedContent, env.IsDevelopment(), cosmosRetryOptions);

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>();
            var policyRegistry = services.AddPolicyRegistry();

            services
                .AddPolicies(policyRegistry, nameof(CareerPathSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<CareerPathSegmentClientOptions>, RefreshSegmentService<CareerPathSegmentClientOptions>, CareerPathSegmentClientOptions>(configuration, nameof(CareerPathSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(CurrentOpportunitiesSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions>, RefreshSegmentService<CurrentOpportunitiesSegmentClientOptions>, CurrentOpportunitiesSegmentClientOptions>(configuration, nameof(CurrentOpportunitiesSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(HowToBecomeSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<HowToBecomeSegmentClientOptions>, RefreshSegmentService<HowToBecomeSegmentClientOptions>, HowToBecomeSegmentClientOptions>(configuration, nameof(HowToBecomeSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(OverviewBannerSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<OverviewBannerSegmentClientOptions>, RefreshSegmentService<OverviewBannerSegmentClientOptions>, OverviewBannerSegmentClientOptions>(configuration, nameof(OverviewBannerSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(RelatedCareersSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<RelatedCareersSegmentClientOptions>, RefreshSegmentService<RelatedCareersSegmentClientOptions>, RelatedCareersSegmentClientOptions>(configuration, nameof(RelatedCareersSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(WhatItTakesSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<WhatItTakesSegmentClientOptions>, RefreshSegmentService<WhatItTakesSegmentClientOptions>, WhatItTakesSegmentClientOptions>(configuration, nameof(WhatItTakesSegmentClientOptions), nameof(PolicyOptions.HttpRetry));

            services
                .AddPolicies(policyRegistry, nameof(WhatYouWillDoSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<WhatYouWillDoSegmentClientOptions>, RefreshSegmentService<WhatYouWillDoSegmentClientOptions>, WhatYouWillDoSegmentClientOptions>(configuration, nameof(WhatYouWillDoSegmentClientOptions), nameof(PolicyOptions.HttpRetry));
        }
    }
}