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
        protected CacheLoader(
            ILogger<CacheLoader> logger,
            AutoMapper.IMapper mapper,
            IProvideSafeOperations safeOperation,
            IProvideGraphContent graphContent)
        {
            Logger = logger;
            GraphContent = graphContent;
            Mapper = mapper;
            SafeOperation = safeOperation;
        }

        internal ILogger<CacheLoader> Logger { get; }

        internal AutoMapper.IMapper Mapper { get; }

        internal IProvideGraphContent GraphContent { get; }

        internal IProvideSafeOperations SafeOperation { get; }

        Task ILoadCacheData.Reload(CancellationToken stoppingToken) =>
            SafeOperation.Try(() => LoadJobProfileCache(stoppingToken), x => LoadError(x));

        public abstract Task LoadJobProfileCache(CancellationToken stoppingToken);

        internal Task LoadError(Exception exception)
        {
            Logger.LogError(exception, "Error in cache reload");
            return Task.CompletedTask;
        }
    }
}
