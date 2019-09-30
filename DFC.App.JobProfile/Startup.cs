using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.HttpClientPolicies;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.App.JobProfile.Repository.CosmosDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DFC.App.JobProfile
{
    public class Startup
    {
        public const string CosmosDbConfigAppSettings = "Configuration:CosmosDbConnections:JobProfile";
        public const string BrandingAssetsConfigAppSettings = "BrandingAssets";

        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var cosmosDbConnection = configuration.GetSection(CosmosDbConfigAppSettings).Get<CosmosDbConnection>();
            var documentClient = new DocumentClient(new Uri(cosmosDbConnection.EndpointUrl), cosmosDbConnection.AccessKey);
            var brandingAssetsModel = configuration.GetSection(BrandingAssetsConfigAppSettings).Get<BrandingAssetsModel>();

            services.AddSingleton(cosmosDbConnection);
            services.AddSingleton<IDocumentClient>(documentClient);
            services.AddSingleton(brandingAssetsModel);
            services.AddSingleton<ICosmosRepository<JobProfileModel>, CosmosRepository<JobProfileModel>>();
            services.AddScoped<IJobProfileService, JobProfileService>();
            services.AddScoped<IDraftJobProfileService, DraftJobProfileService>();
            services.AddScoped<ISegmentService, SegmentService>();

            services.AddSingleton(configuration.GetSection(nameof(CareerPathSegmentClientOptions)).Get<CareerPathSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(CurrentOpportunitiesSegmentClientOptions)).Get<CurrentOpportunitiesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(HowToBecomeSegmentClientOptions)).Get<HowToBecomeSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(OverviewBannerSegmentClientOptions)).Get<OverviewBannerSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(RelatedCareersSegmentClientOptions)).Get<RelatedCareersSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatItTakesSegmentClientOptions)).Get<WhatItTakesSegmentClientOptions>());
            services.AddSingleton(configuration.GetSection(nameof(WhatYouWillDoSegmentClientOptions)).Get<WhatYouWillDoSegmentClientOptions>());

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>();
            var policyRegistry = services.AddPolicyRegistry();

            services
                .AddPolicies(policyRegistry, nameof(CareerPathSegmentClientOptions), policyOptions)
                .AddHttpClient<ICareerPathSegmentService, CareerPathSegmentService, CareerPathSegmentClientOptions>(configuration, nameof(CareerPathSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(CurrentOpportunitiesSegmentClientOptions), policyOptions)
                .AddHttpClient<ICurrentOpportunitiesSegmentService, CurrentOpportunitiesSegmentService, CurrentOpportunitiesSegmentClientOptions>(configuration, nameof(CurrentOpportunitiesSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(HowToBecomeSegmentClientOptions), policyOptions)
                .AddHttpClient<IHowToBecomeSegmentService, HowToBecomeSegmentService, HowToBecomeSegmentClientOptions>(configuration, nameof(HowToBecomeSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(OverviewBannerSegmentClientOptions), policyOptions)
                .AddHttpClient<IOverviewBannerSegmentService, OverviewBannerSegmentService, OverviewBannerSegmentClientOptions>(configuration, nameof(OverviewBannerSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(RelatedCareersSegmentClientOptions), policyOptions)
                .AddHttpClient<IRelatedCareersSegmentService, RelatedCareersSegmentService, RelatedCareersSegmentClientOptions>(configuration, nameof(RelatedCareersSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(WhatItTakesSegmentClientOptions), policyOptions)
                .AddHttpClient<IWhatItTakesSegmentService, WhatItTakesSegmentService, WhatItTakesSegmentClientOptions>(configuration, nameof(WhatItTakesSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services
                .AddPolicies(policyRegistry, nameof(WhatYouWillDoSegmentClientOptions), policyOptions)
                .AddHttpClient<IWhatYouWillDoSegmentService, WhatYouWillDoSegmentService, WhatYouWillDoSegmentClientOptions>(configuration, nameof(WhatYouWillDoSegmentClientOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
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

            app.UseMvc(routes =>
            {
                // add the site map route
                routes.MapRoute(
                    name: "Sitemap",
                    template: "Sitemap.xml",
                    defaults: new { controller = "Sitemap", action = "Sitemap" });

                // add the robots.txt route
                routes.MapRoute(
                    name: "Robots",
                    template: "Robots.txt",
                    defaults: new { controller = "Robot", action = "Robot" });

                // add the default route
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Profile}/{action=Index}");
            });

            mapper?.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
