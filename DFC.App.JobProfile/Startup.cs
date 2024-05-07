using AutoMapper;
using CorrelationId;
using DFC.App.JobProfile.AVService;
using DFC.App.JobProfile.ClientHandlers;
using DFC.App.JobProfile.Data.Configuration;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.HttpClientPolicies.Polly;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.HostedServices;
using DFC.App.JobProfile.HttpClientPolicies;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.App.JobProfile.ProfileService.SegmentServices;
using DFC.App.JobProfile.Repository.CosmosDb;
using DFC.App.JobProfile.Services;
using DFC.Common.SharedContent.Pkg.Netcore;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure;
using DFC.Common.SharedContent.Pkg.Netcore.Infrastructure.Strategy;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Common.SharedContent.Pkg.Netcore.RequestHandler;
using DFC.Compui.Cosmos;
using DFC.Compui.Subscriptions.Pkg.Netstandard.Extensions;
using DFC.Compui.Telemetry;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using DFC.Content.Pkg.Netcore.Services.CmsApiProcessorService;
using DFC.Logger.AppInsights.Extensions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using DFC.FindACourseClient;
using PolicyOptions = DFC.App.JobProfile.HttpClientPolicies.PolicyOptions;
using System.Collections.Generic;
using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;

namespace DFC.App.JobProfile
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public const string StaticCosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:SharedContent";
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfile";
        public const string ConfigAppSettings = "Configuration";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";
        public const string CourseSearchClientSvcSettings = "Configuration:CourseSearchClient:CourseSearchSvc";
        public const string CourseSearchClientAuditSettings = "Configuration:CourseSearchClient:CosmosAuditConnection";
        public const string CourseSearchClientPolicySettings = "Configuration:CourseSearchClient:Policies";
        public const string ServiceBusOptionsAppSettings = "ServiceBusOptions";
        public const string AVAPIServiceAppSettings = "Configuration:AVAPIService";
        public const string AVAPIServiceClientPolicySettings = "Configuration:AVAPIService:Policies";
        private const string StaxGraphApiUrlAppSettings = "Cms:GraphApiUrl";
        private const string RedisCacheConnectionStringAppSettings = "Cms:RedisCacheConnectionString";

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
            services.AddRazorTemplating();
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
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey);

            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton<ICosmosRepository<JobProfileModel>, CosmosRepository<JobProfileModel>>();
            services.AddSingleton<IRedirectionSecurityService, RedirectionSecurityService>();

            services.AddSingleton(configuration.GetSection(nameof(CmsApiClientOptions)).Get<CmsApiClientOptions>() ?? new CmsApiClientOptions());
            var staticContentDbConnection = configuration.GetSection(StaticCosmosDbConfigAppSettings).Get<DFC.Compui.Cosmos.Contracts.CosmosDbConnection>();
            var cosmosRetryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };
            services.AddDocumentServices<StaticContentItemModel>(staticContentDbConnection, env.IsDevelopment(), cosmosRetryOptions);
            services.AddTransient<ICmsApiService, CmsApiService>();
            services.AddTransient<IStaticContentReloadService, StaticContentReloadService>();
            services.AddTransient<IContentTypeMappingService, ContentTypeMappingService>();

            var configValues = configuration.GetSection(ConfigAppSettings).Get<ConfigValues>();
            services.AddSingleton(configValues);

            services.AddScoped<IJobProfileService, JobProfileService>();
            services.AddScoped<ISegmentService, SegmentService>();
            services.AddTransient<CorrelationIdDelegatingHandler>();
            services.AddDFCLogging(configuration["ApplicationInsights:InstrumentationKey"]);

            services.AddStackExchangeRedisCache(options => { options.Configuration = configuration.GetSection(RedisCacheConnectionStringAppSettings).Get<string>(); });
            services.AddSingleton<IGraphQLClient>(s =>
            {
                var option = new GraphQLHttpClientOptions()
                {
                    EndPoint = new Uri(configuration.GetSection(StaxGraphApiUrlAppSettings).Get<string>() ?? throw new ArgumentNullException()),

                    HttpMessageHandler = new CmsRequestHandler(s.GetService<IHttpClientFactory>(), s.GetService<IConfiguration>(), s.GetService<IHttpContextAccessor>() ?? throw new ArgumentNullException()),
                };
                var client = new GraphQLHttpClient(option, new NewtonsoftJsonSerializer());
                return client;
            });

            services.AddSingleton<ISharedContentRedisInterfaceStrategyWithRedisExpiry<JobProfilesOverviewResponse>, JobProfileOverviewProfileSpecificQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyWithRedisExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>, JobProfileCurrentOpportunitiesGetByUrlStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyWithRedisExpiry<RelatedCareersResponse>, JobProfileRelatedCareersQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyWithRedisExpiry<JobProfileHowToBecomeResponse>, JobProfileHowToBecomeQueryStrategy>();
            services.AddSingleton<ISharedContentRedisInterfaceStrategyWithRedisExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>, JobProfileCurrentOpportunitiesGetByUrlStrategy>();

            services.AddSingleton<ISharedContentRedisInterfaceStrategyFactory, SharedContentRedisStrategyFactory>();

            services.AddScoped<ISharedContentRedisInterface, SharedContentRedis>();

            services.AddSingleton(serviceProvider =>
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.AddProfiles(
                        new List<Profile>
                        {
                            new HealthCheckItemProfile(),
                            new JobProfileMetaDataPatchModelProfile(),
                            new JobProfileModelProfile(),
                            new StaticContentItemModelProfile(),
                            new FindACourseProfile(),
                        });
                }).CreateMapper();
            });
            var courseSearchClientSettings = new CourseSearchClientSettings
            {
                CourseSearchSvcSettings = configuration.GetSection(CourseSearchClientSvcSettings).Get<CourseSearchSvcSettings>() ?? new CourseSearchSvcSettings(),
                CourseSearchAuditCosmosDbSettings = configuration.GetSection(CourseSearchClientAuditSettings).Get<CourseSearchAuditCosmosDbSettings>() ?? new CourseSearchAuditCosmosDbSettings(),
                PolicyOptions = configuration.GetSection(CourseSearchClientPolicySettings).Get<DFC.FindACourseClient.PolicyOptions>() ?? new DFC.FindACourseClient.PolicyOptions(),
            };
            services.AddSingleton(courseSearchClientSettings);
            services.AddScoped<ICourseSearchApiService, CourseSearchApiService>();
            services.AddFindACourseServicesWithoutFaultHandling(courseSearchClientSettings);
            var policyRegistry = services.AddPolicyRegistry();
            services.AddFindACourseTransientFaultHandlingPolicies(courseSearchClientSettings, policyRegistry);
          
            services.AddRazorTemplating();

            services.AddSingleton(configuration.GetSection(nameof(CareerPathSegmentClientOptions)).Get<CareerPathSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(CurrentOpportunitiesSegmentClientOptions)).Get<CurrentOpportunitiesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(FeedbackLinks)).Get<FeedbackLinks>() ?? new FeedbackLinks());
            services.AddSingleton(configuration.GetSection(nameof(HowToBecomeSegmentClientOptions)).Get<HowToBecomeSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(OverviewBannerSegmentClientOptions)).Get<OverviewBannerSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(RelatedCareersSegmentClientOptions)).Get<RelatedCareersSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatItTakesSegmentClientOptions)).Get<WhatItTakesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatYouWillDoSegmentClientOptions)).Get<WhatYouWillDoSegmentClientOptions>());

            services.AddHostedServiceTelemetryWrapper();
            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddTransient<IApiCacheService, ApiCacheService>();

            services.AddHostedService<StaticContentReloadBackgroundService>();
            services.AddTransient<IWebhooksService, WebhooksService>();
            services.AddSubscriptionBackgroundService(configuration);

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>();

            var serviceBusOptions = configuration.GetSection(ServiceBusOptionsAppSettings).Get<ServiceBusOptions>();
            var topicClient = new TopicClient(serviceBusOptions.ServiceBusConnectionString, serviceBusOptions.TopicName);
            services.AddSingleton<ITopicClient>(topicClient);

            var avPolicyOptions = configuration.GetSection(AVAPIServiceClientPolicySettings).Get<CorePolicyOptions>();
            avPolicyOptions.HttpRateLimitRetry ??= new RateLimitPolicyOptions();
            policyRegistry.AddRateLimitPolicy(nameof(RefreshClientOptions), avPolicyOptions.HttpRateLimitRetry);
            policyRegistry.AddStandardPolicies(nameof(RefreshClientOptions), avPolicyOptions);

            var aVAPIServiceSettings = configuration.GetSection(AVAPIServiceAppSettings).Get<AVAPIServiceSettings>();
            services.AddSingleton(aVAPIServiceSettings ?? new AVAPIServiceSettings());
            services.BuildHttpClient<IApprenticeshipVacancyApi, ApprenticeshipVacancyApi, RefreshClientOptions>(configuration, nameof(RefreshClientOptions))
            .AddPolicyHandlerFromRegistry($"{nameof(RefreshClientOptions)}_{nameof(CorePolicyOptions.HttpRateLimitRetry)}")
            .AddPolicyHandlerFromRegistry($"{nameof(RefreshClientOptions)}_{nameof(CorePolicyOptions.HttpRetry)}")
            .AddPolicyHandlerFromRegistry($"{nameof(RefreshClientOptions)}_{nameof(CorePolicyOptions.HttpCircuitBreaker)}");

            services.AddScoped<IAVAPIService, AVAPIService>();

            services
                .AddPolicies(policyRegistry, nameof(CareerPathSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<CareerPathSegmentClientOptions>, RefreshSegmentService<CareerPathSegmentClientOptions>, CareerPathSegmentClientOptions>(configuration, nameof(CareerPathSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(CurrentOpportunitiesSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions>, RefreshSegmentService<CurrentOpportunitiesSegmentClientOptions>, CurrentOpportunitiesSegmentClientOptions>(configuration, nameof(CurrentOpportunitiesSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(HowToBecomeSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<HowToBecomeSegmentClientOptions>, RefreshSegmentService<HowToBecomeSegmentClientOptions>, HowToBecomeSegmentClientOptions>(configuration, nameof(HowToBecomeSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(OverviewBannerSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<OverviewBannerSegmentClientOptions>, RefreshSegmentService<OverviewBannerSegmentClientOptions>, OverviewBannerSegmentClientOptions>(configuration, nameof(OverviewBannerSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(RelatedCareersSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<RelatedCareersSegmentClientOptions>, RefreshSegmentService<RelatedCareersSegmentClientOptions>, RelatedCareersSegmentClientOptions>(configuration, nameof(RelatedCareersSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(WhatItTakesSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<WhatItTakesSegmentClientOptions>, RefreshSegmentService<WhatItTakesSegmentClientOptions>, WhatItTakesSegmentClientOptions>(configuration, nameof(WhatItTakesSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(WhatYouWillDoSegmentClientOptions), policyOptions)
                .AddHttpClient<ISegmentRefreshService<WhatYouWillDoSegmentClientOptions>, RefreshSegmentService<WhatYouWillDoSegmentClientOptions>, WhatYouWillDoSegmentClientOptions>(configuration, nameof(WhatYouWillDoSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services.AddHealthChecks().AddCheck<AVAPIService>("Apprenticeship Service");
        }
    }
}