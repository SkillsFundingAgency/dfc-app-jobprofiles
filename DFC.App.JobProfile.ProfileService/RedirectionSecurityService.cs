using DFC.App.JobProfile.Data.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace DFC.App.JobProfile.ProfileService
{
    public class RedirectionSecurityService : IRedirectionSecurityService
    {
        private readonly string[] redirectionHostWhitelist;

        public RedirectionSecurityService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var allowedWhitelistValues = configuration.GetValue<string>("AllowedRedirects");

            if (!string.IsNullOrEmpty(allowedWhitelistValues))
            {
                redirectionHostWhitelist = allowedWhitelistValues.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            }
        }

        public bool IsValidHost(Uri host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            return host.IsLoopback || host.Host.Split(".").Any(s => redirectionHostWhitelist.Contains(s.ToLower()));
        }
    }
}
