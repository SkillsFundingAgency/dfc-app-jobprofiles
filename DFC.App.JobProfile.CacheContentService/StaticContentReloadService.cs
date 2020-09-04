using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using dfc_content_pkg_netcore.contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CacheContentService
{
    public class StaticContentReloadService : IStaticContentReloadService
    {
        private readonly ILogger<StaticContentReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<StaticContentItemModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;

        public StaticContentReloadService(
            ILogger<StaticContentReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<StaticContentItemModel> eventMessageService,
            ICmsApiService cmsApiService,
            IContentCacheService contentCacheService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.contentCacheService = contentCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload static content started");

                var staticContent = await cmsApiService.GetContentAsync<StaticContentItemModel>().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload static content cancelled");

                    return;
                }

                if (staticContent != null)
                {
                    await ProcessContentAsync(staticContent, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("Reload static content cancelled");

                        return;
                    }
                }

                logger.LogInformation("Reload static content completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in static content reload");
            }
        }

        public async Task ProcessContentAsync(List<StaticContentItemModel> sharedContent, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            contentCacheService.Clear();

            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Process summary list cancelled");

                return;
            }

            await GetAndSaveItemAsync(sharedContent, stoppingToken).ConfigureAwait(false);

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(List<StaticContentItemModel> items, CancellationToken stoppingToken)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            foreach (var item in items) {
                item.PartitionKey = "/";
                item.CanonicalName = item.skos_prefLabel.Replace(" ", "").ToLower();
                try
                {
                    logger.LogInformation($"Updating static content cache with {item.Id} - {item.Url}");

                    var result = await eventMessageService.UpdateAsync(item).ConfigureAwait(false);

                    if (result == HttpStatusCode.NotFound)
                    {
                        logger.LogInformation($"Does not exist, creating static content cache with {item.Id} - {item.Url}");

                        result = await eventMessageService.CreateAsync(item).ConfigureAwait(false);

                        if (result == HttpStatusCode.Created)
                        {
                            logger.LogInformation($"Created static content cache with {item.Id} - {item.Url}");
                        }
                        else
                        {
                            logger.LogError($"Static content cache create error status {result} from {item.Id} - {item.Url}");
                        }
                    }
                    else
                    {
                        logger.LogInformation($"Updated static content cache with {item.Id} - {item.Url}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error in get and save for {item.Id} - {item.Url}");
                }
            }
        }
    }
}
