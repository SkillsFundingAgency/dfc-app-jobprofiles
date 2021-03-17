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
    internal sealed partial class JobProfileCacheLoader :
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

            foreach (var item in summaryList.OrderByDescending(o => o.Published))
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

        internal string ConjunctiveMapping(string candidate, string title) =>
            candidate switch
            {
                "as_defined" => title.ToLowerInvariant(),
                "no_title" => string.Empty,
                "no_prefix" => title.ToLowerInvariant(),
                "prefix_with_a" => $"a {title.ToLowerInvariant()}",
                "prefix_with_an" => $"an {title.ToLowerInvariant()}",
                _ => throw new NotImplementedException()
            };

        internal ContentApiRootElement OrganiseSegments(ContentApiRootElement apiDataModel)
        {
            var contentItems = apiDataModel.ContentItems.Flatten(s => s.ContentItems);

            // how to become (root items)
            var moreInfo = new ContentApiHowToBecomeMoreInformation();
            var howToBecome = new ContentApiHowToBecome();

            howToBecome.Title = ConjunctiveMapping(apiDataModel.TitleOptions, apiDataModel.Title);

            moreInfo.CareerTips = apiDataModel.HowToBecomeCareerTips;
            moreInfo.ProfessionalBodies = apiDataModel.HowToBecomeProfessionalBodies;
            moreInfo.FurtherInformation = apiDataModel.HowToBecomeFurtherInformation;

            howToBecome.MoreInformation = moreInfo;
            apiDataModel.HowToBecome = howToBecome;

            // what it takes (root items)
            var whatItTakes = new ContentApiWhatItTakes();

            apiDataModel.WhatItTakes = whatItTakes;

            // what you'll do (root items)
            var whatYouWillDo = new ContentApiWhatYouWillDo();

            whatYouWillDo.DayToDayTasks = apiDataModel.WhatYouWillDoDayToDayTasks;

            apiDataModel.WhatYouWillDo = whatYouWillDo;

            // career path (root items)
            var careerPathSegment = new ContentApiCareerPath();
            careerPathSegment.CareerPathAndProgression = apiDataModel.CareerPathAndProgression;

            apiDataModel.CareerPath = careerPathSegment;

            if (!contentItems.Any())
            {
                Logger.LogError($"No Content Items for '{apiDataModel.CanonicalName}'");
                return apiDataModel;
            }

            // root element (content items)
            apiDataModel.RelatedCareers = Make5RelatedCareers(contentItems);

            // how to become (content items)
            howToBecome.DirectRoute = GetGeneralRoute(contentItems, "Direct");
            howToBecome.OtherRoute = GetGeneralRoute(contentItems, "Other");
            howToBecome.VolunteeringRoute = GetGeneralRoute(contentItems, "Volunteering");
            howToBecome.WorkRoute = GetGeneralRoute(contentItems, "Work");

            howToBecome.ApprenticeshipRoute = GetEducationalRoute(contentItems, "Apprenticeship");
            howToBecome.CollegeRoute = GetEducationalRoute(contentItems, "College");
            howToBecome.UniversityRoute = GetEducationalRoute(contentItems, "University");

            moreInfo.Registration = GetDescription(contentItems, "Registration");

            // what it takes (content items)
            var witAddition = GetDescription(contentItems, "DigitalSkillsLevel");
            whatItTakes.Restrictions = GetDescriptions(contentItems, "Restriction");
            whatItTakes.OtherRequirements = GetDescriptions(contentItems, "OtherRequirement");
            whatItTakes.Skills = GetDescriptions(contentItems, "ONetSkill")
                .Concat(new string[] { witAddition })
                .AsSafeReadOnlyList();

            // what you'll do (content items)
            whatYouWillDo.WorkingEnvironment = GetDescriptions(contentItems, "WorkingEnvironment");
            whatYouWillDo.WorkingLocation = GetDescriptions(contentItems, "WorkingLocation");
            whatYouWillDo.WorkingUniform = GetDescriptions(contentItems, "WorkingUniform");

            // career path (content items)
            //careerPathSegment.ApprenticeshipStandard = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipStandard").ToList();

            return apiDataModel;
        }

        internal IReadOnlyCollection<ContentApiBranchElement> GetContentItems(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .ToList();

        internal string GetDescription(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetDescriptions(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetDescriptions(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetContentItems(branches, contentType)
                .Select(x => x.Description)
                .ToList();

        internal string GetRawText(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetRawTexts(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetRawTexts(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetContentItems(branches, contentType)
                .Select(x => x.Title)
                .ToList();

        internal string GetText(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetTexts(branches, contentType).FirstOrDefault();

        internal IReadOnlyCollection<string> GetTexts(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetContentItems(branches, contentType)
                .Select(x => x.Text)
                .ToList();

        internal IReadOnlyCollection<ApiAnchor> GetAnchorLinks(IEnumerable<ContentApiBranchElement> branches, string contentType) =>
            GetContentItems(branches, contentType)
                .Select(x => new ApiAnchor { Text = x.LinkText, Link = x.Link })
                .ToList();

        internal IReadOnlyCollection<ApiAnchor> Make5RelatedCareers(IEnumerable<ContentApiBranchElement> branches) =>
            GetContentItems(branches, "JobProfile")
                .Take(5)
                .Select(x => new ApiAnchor { Text = x.Title, Link = $"/job-profiles/{x.Title.ToLowerInvariant().Replace(" ", "-")}" })
                .ToList();

        internal ApiEducationalRoute GetEducationalRoute(IEnumerable<ContentApiBranchElement> branches, string contentTopic)
        {
            var content = branches.FirstOrDefault(x => x.ContentType == $"{contentTopic}Route");

            if (content == null)
            {
                return null;
            }

            var route = new ApiEducationalRoute();
            var item = new ApiEducationalRouteItem();

            route.Topic = contentTopic;
            route.RelevantSubjects = content.RelevantSubjects;
            route.FurtherInformation = content.FurtherInformation;
            route.RequirementsAndReading = item;

            //item.Preface = GetDescription(branches, "RequirementsPrefix");
            item.Requirements = GetTexts(branches, $"{contentTopic}Requirement");
            item.FurtherReading = GetAnchorLinks(branches, $"{contentTopic}Link");

            return route;
        }

        internal ApiGeneralRoute GetGeneralRoute(IEnumerable<ContentApiBranchElement> branches, string contentTopic)
        {
            if (!branches.SafeAny(x => x.ContentType == $"{contentTopic}Route"))
            {
                return null;
            }

            var route = new ApiGeneralRoute();

            route.Topic = contentTopic;
            route.Descriptions = GetDescriptions(branches, $"{contentTopic}Route");

            return route;
        }
    }
}
