using DFC.App.JobProfile.MessageFunctionApp.HttpClientPolicies;
using DFC.Functions.DI.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

[assembly: WebJobsStartup(typeof(DFC.App.JobProfile.MessageFunctionApp.Startup.WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.App.JobProfile.MessageFunctionApp.Startup
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            var jobProfileClientOptions = configuration.GetSection("jobProfileClientOptions").Get<JobProfileClientOptions>();

            builder.AddDependencyInjection();

            builder.Services.AddSingleton<JobProfileClientOptions>(jobProfileClientOptions);
            builder.Services.AddSingleton<HttpClient>(new HttpClient());
        }
    }
}
