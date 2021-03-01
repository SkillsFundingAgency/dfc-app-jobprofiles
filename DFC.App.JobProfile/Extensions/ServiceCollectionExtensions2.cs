// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
//using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using System.Net.Http;

//namespace DFC.App.JobProfile.Extensions
//{
//    public static class ServiceCollectionExtensions2
//    {
//        public static IServiceCollection AddHttpClienthh<TClient, TImplementation, TClientOptions>(
//                    this IServiceCollection services,
//                    IConfiguration configuration,
//                    string configurationSectionName,
//                    string retryPolicyName,
//                    string circuitBreakerPolicyName)
//                    where TClient : class
//                    where TImplementation : class, TClient
//                    where TClientOptions : ClientOptionsModel, new() =>
//                    services
//                        .Configure<TClientOptions>(configuration?.GetSection(configurationSectionName))
//                        .AddHttpClient<TClient, TImplementation>()
//                        .ConfigureHttpClient((sp, options) =>
//                        {
//                            var httpClientOptions = sp
//                                .GetRequiredService<IOptions<TClientOptions>>()
//                                .Value;
//                            options.BaseAddress = httpClientOptions.BaseAddress;
//                            options.Timeout = httpClientOptions.Timeout;

//                            if (!string.IsNullOrWhiteSpace(httpClientOptions.ApiKey))
//                            {
//                                options.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", httpClientOptions.ApiKey);
//                            }
//                        })
//                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
//                        {
//                            AllowAutoRedirect = false,
//                        })
//                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{retryPolicyName}")
//                        .AddPolicyHandlerFromRegistry($"{configurationSectionName}_{circuitBreakerPolicyName}")
//                        .Services;
//    }
//}