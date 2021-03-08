// TODO: content cache service (other cache thingy) don't understand what this does...
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.EventProcessing.Services;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    internal sealed class JobProfileCacheLoader :
        CacheLoader,
        ILoadJobProfileContent,
        IRequireServiceRegistration
    {
        private readonly IEventMessageService<JobProfileCached> _messageService;

        public JobProfileCacheLoader(
            ILogger<JobProfileCacheLoader> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<JobProfileCached> eventMessageService,
            IProvideSafeOperations safeOperation,
            IProvideGraphContent graphContent)
            : base(logger, mapper, safeOperation, graphContent)
        {
            _messageService = eventMessageService;
        }

        public override async Task Load(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Reload cache started, get summary list");

            var summaryList = await GraphContent.GetSummaryItems<ContentApiSummaryItem>();

            Logger.LogInformation("Get summary list completed");

            if (summaryList.Any())
            {
                await ProcessSummaryList(summaryList, stoppingToken);

                await DeleteStaleCacheEntries(summaryList, stoppingToken);
            }

            Logger.LogInformation("Reload cache completed");
        }

        internal async Task ProcessSummaryList(
            IReadOnlyCollection<IGraphSummaryItem> summaryList,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation("Process summary list started");

            //_otherCacheThingy.Clear();

            foreach (var item in summaryList.OrderBy(o => o.Published))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Process summary list cancelled");
                    return;
                }

                await GetAndSaveItem(item);
            }

            Logger.LogInformation("Process summary list completed");
        }

        internal async Task GetAndSaveItem(IGraphSummaryItem item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            Logger.LogInformation($"Get details for {item.CanonicalName} - {item.Uri}");

            var apiDataModel = await GraphContent
                .GetComposedItem<ContentApiRootElement, ContentApiBranchElement>(item.Uri);

            if (apiDataModel == null || apiDataModel.IsFaultedState())
            {
                Logger.LogWarning($"No details returned from {item.CanonicalName} - {item.Uri}");
                return;
            }

            apiDataModel = OrganiseSegments(apiDataModel);
            var contentPageModel = Mapper.Map<JobProfileCached>(apiDataModel);

            Logger.LogInformation($"Updating cache with {item.CanonicalName} - {item.Uri}");

            var result = await _messageService.UpdateAsync(contentPageModel);

            if (result == HttpStatusCode.NotFound)
            {
                Logger.LogInformation($"Does not exist, creating cache with {item.CanonicalName} - {item.Uri}");

                result = await _messageService.CreateAsync(contentPageModel);

                if (result != HttpStatusCode.Created)
                {
                    Logger.LogError($"Cache create error status {result} from {item.CanonicalName} - {item.Uri}");
                }
            }

            //var contentItemIds = contentPageModel.AllContentItemIds.ToList();
            //_otherCacheThingy.AddOrReplace(contentPageModel.Id, contentItemIds);
        }

        internal async Task DeleteStaleCacheEntries(
            IReadOnlyCollection<IGraphSummaryItem> summaryList,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = (await _messageService.GetAllCachedCanonicalNamesAsync()).AsSafeReadOnlyList();

            if (cachedContentPages.Any())
            {
                var hashedSummaryList = new HashSet<Uri>(summaryList.Select(p => p.Uri));
                var staleContentPages = cachedContentPages.Where(p => !hashedSummaryList.Contains(p.Uri)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItems(staleContentPages, stoppingToken);
                }
            }

            Logger.LogInformation("Delete stale cache items completed");
        }

        internal async Task DeleteStaleItems(
            IReadOnlyCollection<JobProfileCached> staleItems,
            CancellationToken stoppingToken)
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning("Delete stale cache items cancelled");
                    return;
                }

                Logger.LogInformation($"Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await _messageService.DeleteAsync(staleContentPage.Id);

                if (deletionResult != HttpStatusCode.OK)
                {
                    Logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        internal ContentApiRootElement OrganiseSegments(ContentApiRootElement apiDataModel)
        {
            var contentItems = apiDataModel.ContentItems.Flatten(s => s.ContentItems);

            if (!contentItems.Any())
            {
                Logger.LogError($"No Content Items for '{apiDataModel.CanonicalName}'");
                return apiDataModel;
            }

            //What You'll Do
            var whatYoullDoModel = new ContentApiWhatYoullDo();
            whatYoullDoModel.DayToDayTasks = apiDataModel.WhatYoullDoDayToDayTasks;
            whatYoullDoModel.WorkingEnvironment = GetDescriptions(contentItems, "WorkingEnvironment");
            whatYoullDoModel.WorkingLocation = GetDescriptions(contentItems, "WorkingLocation");
            whatYoullDoModel.WorkingUniform = GetDescriptions(contentItems, "WorkingUniform");
            apiDataModel.WhatYoullDo = whatYoullDoModel;

            //Career Path
            var careerPathSegment = new ContentApiCareerPath();

            //careerPathSegment.ApprenticeshipStandard = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipStandard").ToList();

            careerPathSegment.CareerPathAndProgression = apiDataModel.CareerPathAndProgression;
            apiDataModel.CareerPath = careerPathSegment;

            //How to Become
            var moreInfo = new ContentApiHowToBecomeMoreInformation();
            var howToBecomeSegment = new ContentApiHowToBecome();
            howToBecomeSegment.Title = apiDataModel.Title;

            howToBecomeSegment.DirectRoute = GetDescription(contentItems, "DirectRoute");
            howToBecomeSegment.OtherRoute = GetDescription(contentItems, "OtherRoute");
            howToBecomeSegment.VolunteeringRoute = GetDescription(contentItems, "VolunteeringRoute");
            howToBecomeSegment.WorkRoute = GetDescription(contentItems, "WorkRoute");

            howToBecomeSegment.ApprenticeshipRoute = GetRoute(contentItems, "Apprenticeship");
            howToBecomeSegment.CollegeRoute = GetRoute(contentItems, "College");
            howToBecomeSegment.UniversityRoute = GetRoute(contentItems, "University");

            moreInfo.Registration = GetDescription(contentItems, "Registration");
            moreInfo.CareerTips = apiDataModel.HowToBecomeCareerTips;
            moreInfo.ProfessionalBodies = apiDataModel.HowToBecomeProfessionalBodies;
            moreInfo.FurtherInformation = apiDataModel.HowToBecomeFurtherInformation;
            howToBecomeSegment.MoreInformation = moreInfo;
            apiDataModel.HowToBecome = howToBecomeSegment;

            //What it Takes
            var whatItTakes = new ContentApiWhatItTakes();

            //whatItTakes.Restrictions = apiDataModel.ContentItems.Where(x => x.ContentType == "Restriction").ToList();
            //whatItTakes.OtherRequirement = apiDataModel.ContentItems.Where(x => x.ContentType == "OtherRequirement").ToList();

            var witAddition = new List<string> { apiDataModel.WhatItTakesDigitalSkillsLevel };
            whatItTakes.Skills = GetDescriptions(contentItems, "ONetSkill")
                .Concat(witAddition)
                .AsSafeReadOnlyList();

            apiDataModel.WhatItTakes = whatItTakes;

            return apiDataModel;
        }

        internal string GetDescription(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetDescriptions(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetDescriptions(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => x.Description)
                .ToList();

        internal string GetRawText(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetRawTexts(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetRawTexts(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => x.Title)
                .ToList();

        internal string GetText(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetTexts(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetTexts(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => x.Text)
                .ToList();

        internal IReadOnlyCollection<ApiAnchor> GetAnchorLinks(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => new ApiAnchor { Text = x.Text, Link = x.Link })
                .ToList();

        internal ApiEducationalRoute GetRoute(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            var content = branches.FirstOrDefault(x => x.ContentType == $"{contentType}Route");

            if (content == null)
            {
                return null;
            }

            var route = new ApiEducationalRoute();
            var item = new ApiEducationalRouteItem();

            route.MoreInformation = item;
            route.FurtherInformation = content.FurtherInformation;
            route.RelevantSubjects = content.RelevantSubjects;

            item.Preamble = GetDescription(branches, "RequirementsPrefix");
            item.Requirements = GetTexts(branches, $"{contentType}Requirement");
            item.FurtherReading = GetAnchorLinks(branches, $"{contentType}Link");

            return route;
        }
    }
}
