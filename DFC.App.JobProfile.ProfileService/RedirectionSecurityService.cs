using AutoMapper.Configuration;
using DFC.App.JobProfile.Data.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace DFC.App.JobProfile.ProfileService
{
    public class RedirectionSecurityService : IRedirectionSecurityService
    {
        private readonly string[] redirectionHostWhitelist;

        public RedirectionSecurityService(
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            redirectionHostWhitelist = configuration.GetSection("AllowedRedirects").Get<string[]>();
        }

        public bool IsValidHost(Uri host)
        {
            return host.IsLoopback || host.Host.Split(".").Any(s => redirectionHostWhitelist.Contains(s.ToLower()));
        }
    }
}
