using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal sealed class MigratedOpportunitiesLoader :
        CacheLoader,
        IMigrateCurrentOpportunities,
        IRequireServiceRegistration
    {
        private readonly IEventMessageService<CurrentOpportunities> _messageService;
        private readonly IContentPageService<SegmentCurrentOpportunity> _pageService;

        public MigratedOpportunitiesLoader(
            ILogger<MigratedOpportunitiesLoader> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<CurrentOpportunities> eventMessageService,
            IContentPageService<SegmentCurrentOpportunity> pageService,
            IProvideSafeOperations safeOperation,
            IProvideGraphContent graphContent)
            : base(logger, mapper, safeOperation, graphContent)
        {
            _messageService = eventMessageService;
            _pageService = pageService;
        }

        public override async Task Load(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Reload current opportunities commenced");

            var candidates = await _pageService.GetAllAsync().AsSafeReadOnlyList();

            if (candidates.Any())
            {
                await ProcessContentAsync(candidates, stoppingToken);
            }

            Logger.LogInformation("Reload static content completed");
        }

        public async Task ProcessContentAsync(
            IReadOnlyCollection<SegmentCurrentOpportunity> candidates,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation("Process summary list started");

            foreach (var item in candidates)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Process summary list cancelled");
                    return;
                }

                var candidate = Mapper.Map<CurrentOpportunities>(item);

                Logger.LogInformation($"Updating current opportunity cache with {candidate.Id} - {candidate.CanonicalName}");

                var result = await _messageService.UpdateAsync(candidate);

                if (result == HttpStatusCode.NotFound)
                {
                    Logger.LogInformation($"Does not exist, creating static content cache with {candidate.Id} - {candidate.CanonicalName}");

                    result = await _messageService.CreateAsync(candidate);

                    if (result == HttpStatusCode.OK)
                    {
                        Logger.LogInformation($"Created static content cache with {candidate.Id} - {candidate.CanonicalName}");
                    }
                    else
                    {
                        Logger.LogError($"Static content cache create error status {result} from {candidate.Id} - {candidate.CanonicalName}");
                    }
                }
                else
                {
                    Logger.LogInformation($"Updated static content cache with {candidate.Id} - {candidate.CanonicalName}");
                }
            }
        }
    }
}
