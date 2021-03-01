// TODO: content cache service (other cache thingy) don't understand what this does...
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal abstract class CacheLoader :
        ILoadCacheData,
        IRequireServiceRegistration
    {
        //private readonly IContentCacheService _otherCacheThingy;

        protected CacheLoader(
            ILogger<CacheLoader> logger,
            AutoMapper.IMapper mapper,
            IProvideSafeOperations safeOperation,
            IProvideGraphContent graphContent)
        //,IContentCacheService otherCacheThingy)
        {
            Logger = logger;
            GraphContent = graphContent;
            Mapper = mapper;
            SafeOperation = safeOperation;
            //_otherCacheThingy = otherCacheThingy;
        }

        internal ILogger<CacheLoader> Logger { get; }

        internal AutoMapper.IMapper Mapper { get; }

        internal IProvideGraphContent GraphContent { get; }

        internal IProvideSafeOperations SafeOperation { get; }

        public async Task Reload(CancellationToken stoppingToken) =>
            await SafeOperation.Try(() => Load(stoppingToken), x => LoadError(x));

        public abstract Task Load(CancellationToken stoppingToken);

        internal async Task LoadError(Exception exception)
        {
            Logger.LogError(exception, "Error in cache reload");
            await Task.CompletedTask;
        }
    }
}
