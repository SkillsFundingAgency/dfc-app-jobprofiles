// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.HttpClientPolicies;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System;

namespace DFC.App.JobProfile.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPolicies(
            this IServiceCollection services,
            IPolicyRegistry<string> policyRegistry,
            string keyPrefix,
            PolicyOptions policyOptions)
        {
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

        //public static IServiceCollection AddHttpClient<TClient, TImplementation, TClientOptions>(
        //            this IServiceCollection services,
        //            IConfiguration configuration,
        //            string configurationSectionName,
        //            string retryPolicyName,
        //            string circuitBreakerPolicyName)
        //            where TClient : class
        //            where TImplementation : class, TClient
        //            where TClientOptions : SegmentClientOptions, new() =>
        //            services
        //                .Configure<TClientOptions>(configuration?.GetSection(configurationSectionName))
        //                .AddHttpClient<TClient, TImplementation>()
        //                .ConfigureHttpClient((sp, options) =>
        //                {
        //                    var httpClientOptions = sp
        //                        .GetRequiredService<IOptions<TClientOptions>>()
        //                        .Value;
        //                    options.BaseAddress = httpClientOptions.BaseAddress;
        //                    options.Timeout = httpClientOptions.Timeout;
        //                    options.DefaultRequestHeaders.Clear();
        //                    options.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        //                })
        //                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
        //                {
        //                    AllowAutoRedirect = false,
        //                })
        //                .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
        //                .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
        //                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
        //                .Services;
    }
}