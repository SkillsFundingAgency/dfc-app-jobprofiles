// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

            var staticContent = await GraphContent.GetStaticItems<ContentApiStaticElement>().ConfigureAwait(false);

            if (stoppingToken.IsCancellationRequested)
            {
                Logger.LogWarning("Reload static content cancelled");

                return;
            }

            if (staticContent != null)
            {
                await ProcessContentAsync(staticContent, stoppingToken).ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Reload static content cancelled");

                    return;
                }
            }

            Logger.LogInformation("Reload static content completed");
        }

        public async Task ProcessContentAsync(
            IReadOnlyCollection<ContentApiStaticElement> sharedContent,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation("Process summary list started");

            if (stoppingToken.IsCancellationRequested)
            {
                Logger.LogWarning("Process summary list cancelled");

                return;
            }

            await GetAndSaveItemAsync(sharedContent, stoppingToken).ConfigureAwait(false);

            Logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(
            IReadOnlyCollection<ContentApiStaticElement> items,
            CancellationToken stoppingToken)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                var candidate = Mapper.Map<StaticItemCached>(item);

                candidate.CanonicalName = item.Title.Replace(" ", "-").ToLower();

                Logger.LogInformation($"Updating static content cache with {candidate.Id} - {candidate.Uri}");

                var result = await _messageService.UpdateAsync(candidate).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    Logger.LogInformation($"Does not exist, creating static content cache with {candidate.Id} - {candidate.Uri}");

                    result = await _messageService.CreateAsync(candidate).ConfigureAwait(false);

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
