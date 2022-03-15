using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.App.JobProfile.MessageFunctionApp.Services;
using DFC.Functions.DI.Standard;
using DFC.Logger.AppInsights.Contracts;
using DFC.Logger.AppInsights.CorrelationIdProviders;
using DFC.Logger.AppInsights.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(DFC.App.JobProfile.MessageFunctionApp.Startup.WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfile.MessageFunctionApp.Startup
{
    [ExcludeFromCodeCoverage]
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            builder?.AddDependencyInjection();
            builder?.Services.AddSingleton(configuration.GetSection("jobProfileClientOptions").Get<JobProfileClientOptions>());
            builder?.Services.AddHttpClient();
            builder?.Services.AddAutoMapper(typeof(WebJobsExtensionStartup).Assembly);
            builder?.Services.AddScoped<IMessageProcessor, MessageProcessor>();
            builder?.Services.AddScoped<IHttpClientService<JobProfileModel>, HttpClientService<JobProfileModel>>();
            builder?.Services.AddDFCLogging(configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
            builder?.Services.AddScoped<ICorrelationIdProvider, InMemoryCorrelationIdProvider>();
        }
    }
}