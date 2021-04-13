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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    [ExcludeFromCodeCoverage]
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

        public override async Task LoadJobProfileCache(CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Reload current opportunities commenced");

            var candidates = await _pageService.GetAllAsync().AsSafeReadOnlyList();

            if (candidates.Any())
            {
                await ProcessContentAsync(candidates, stoppingToken);
            }

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Reload static content completed");
        }

        public async Task ProcessContentAsync(
            IReadOnlyCollection<SegmentCurrentOpportunity> candidates,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Process summary list started");

            foreach (var item in candidates)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning($"{Utils.LoggerMethodNamePrefix()} Process summary list cancelled");
                    return;
                }

                var candidate = Mapper.Map<CurrentOpportunities>(item);

                Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Updating current opportunity cache with {candidate.Id} - {candidate.CanonicalName}");

                var result = await _messageService.UpdateAsync(candidate);

                if (result == HttpStatusCode.NotFound)
                {
                    Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Does not exist, creating static content cache with {candidate.Id} - {candidate.CanonicalName}");

                    result = await _messageService.CreateAsync(candidate);

                    if (result == HttpStatusCode.OK)
                    {
                        Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Created static content cache with {candidate.Id} - {candidate.CanonicalName}");
                    }
                    else
                    {
                        Logger.LogError($"{Utils.LoggerMethodNamePrefix()} Static content cache create error status {result} from {candidate.Id} - {candidate.CanonicalName}");
                    }
                }
                else
                {
                    Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Updated static content cache with {candidate.Id} - {candidate.CanonicalName}");
                }
            }
        }
    }
}
