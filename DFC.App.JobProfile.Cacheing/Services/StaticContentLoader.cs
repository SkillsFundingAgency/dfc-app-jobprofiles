using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal sealed class StaticContentLoader :
        CacheLoader,
        ILoadStaticContent,
        IRequireServiceRegistration
    {
        private readonly IEventMessageService<StaticItemCached> _messageService;

        public StaticContentLoader(
            ILogger<StaticContentLoader> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<StaticItemCached> eventMessageService,
            IProvideSafeOperations safeOperation,
            IProvideGraphContent graphContent)
            : base(logger, mapper, safeOperation, graphContent)
        {
            _messageService = eventMessageService;
        }

        public override async Task Load(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Reload static content started");

            var staticItems = await GraphContent.GetStaticItems<ContentApiStaticElement>();

            if (staticItems.Any())
            {
                await ProcessContentAsync(staticItems, stoppingToken);
            }

            Logger.LogInformation("Reload static content completed");
        }

        public async Task ProcessContentAsync(
            IReadOnlyCollection<ContentApiStaticElement> sharedItems,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation("Process summary list started");

            foreach (var item in sharedItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Process summary list cancelled");
                    return;
                }

                var candidate = Mapper.Map<StaticItemCached>(item);

                candidate.CanonicalName = item.Title.Replace(" ", "-").ToLower();

                Logger.LogInformation($"Updating static content cache with {candidate.Id} - {candidate.Uri}");

                var result = await _messageService.UpdateAsync(candidate);

                if (result == HttpStatusCode.NotFound)
                {
                    Logger.LogInformation($"Does not exist, creating static content cache with {candidate.Id} - {candidate.Uri}");

                    result = await _messageService.CreateAsync(candidate);

                    if (result == HttpStatusCode.OK)
                    {
                        Logger.LogInformation($"Created static content cache with {candidate.Id} - {candidate.Uri}");
                    }
                    else
                    {
                        Logger.LogError($"Static content cache create error status {result} from {candidate.Id} - {candidate.Uri}");
                    }
                }
                else
                {
                    Logger.LogInformation($"Updated static content cache with {candidate.Id} - {candidate.Uri}");
                }
            }
        }
    }
}
