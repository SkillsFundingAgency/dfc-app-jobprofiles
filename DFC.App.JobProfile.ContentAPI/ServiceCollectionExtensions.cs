using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Content.Pkg.Netcore.Data.Models.PollyOptions;
using DFC.Content.Pkg.Netcore.Services;
using DFC.Content.Pkg.Netcore.Services.ApiProcessorService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;
using System.Net.Http;

namespace DFC.App.JobProfile.ContentAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IPolicyRegistry<string> policyRegistry)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));

            services.AddTransient<IApiService, ApiService>();
            services.AddTransient<IApiDataProcessorService, ApiDataProcessorService>();
            services.AddSingleton<IApiCacheService, ApiCacheService>();

            const string AppSettingsPolicies = "Policies";
            var policyOptions = configuration.GetSection(AppSettingsPolicies).Get<PolicyOptions>() ?? new PolicyOptions();

            services
                .AddPolicies(policyRegistry, nameof(ContentApiOptions), policyOptions)
                .AddHttpClient<IProvideGraphContent, GraphContentProvider, ContentApiOptions>(configuration, nameof(ContentApiOptions), nameof(PolicyOptions.HttpRetry), nameof(PolicyOptions.HttpCircuitBreaker));

            return services;
        }

        public static IServiceCollection AddPolicies(
           this IServiceCollection services,
           IPolicyRegistry<string> policyRegistry,
           string keyPrefix,
           PolicyOptions policyOptions)
        {
            _ = policyOptions ?? throw new ArgumentNullException(nameof(policyOptions));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpRetry)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        policyOptions.HttpRetry.Count,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));

            policyRegistry?.Add(
                $"{keyPrefix}_{nameof(PolicyOptions.HttpCircuitBreaker)}",
                HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                        durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

            return services;
        }

        public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationSectionName,
            string retryPolicyName,
            string circuitBreakerPolicyName)
                where TClient : class
                where TImplementation : class, TClient
                where TClientOptions : ClientOptionsModel, new()
        {
            return services
                .Configure<TClientOptions>(options => configuration?.GetSection(configurationSectionName))
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient((sp, options) =>
                {
                    var httpClientOptions = sp
                    .GetRequiredService<IOptions<TClientOptions>>()
                    .Value;
                    options.BaseAddress = httpClientOptions.BaseAddress;
                    options.Timeout = httpClientOptions.Timeout;

                    if (!string.IsNullOrWhiteSpace(httpClientOptions.ApiKey))
                    {
                        options.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", httpClientOptions.ApiKey);
                    }
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    AllowAutoRedirect = false,
                })
                .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
                .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
                .Services;
        }
    }
}