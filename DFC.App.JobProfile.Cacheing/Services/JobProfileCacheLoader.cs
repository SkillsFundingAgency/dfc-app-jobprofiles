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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing.Services
{
    [ExcludeFromCodeCoverage]
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

        public override async Task LoadJobProfileCache(CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Reload cache started, getting list of Job Profile summaries");

            var jobProfileSummaries = await GraphContent.GetSummaryItems<ContentApiSummaryItem>();
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Getting list of job profile summaries completed");

            if (jobProfileSummaries.Any())
            {
                await GetIndividualJobProfileContentItemsAndStoreInCache(jobProfileSummaries, stoppingToken);
                await DeleteStaleCacheEntries(jobProfileSummaries, stoppingToken);
            }

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Reload of Job Profile cache completed");
        }

        internal async Task GetIndividualJobProfileContentItemsAndStoreInCache(
            IReadOnlyCollection<IGraphSummaryItem> jobProfileSummaries,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Iterating job profile summary list started");

            foreach (var jobProfileSummary in jobProfileSummaries.OrderByDescending(o => o.ModifiedDateTime))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning($"{Utils.LoggerMethodNamePrefix()} Iterating job profile summary list cancelled");

                    return;
                }

                Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Getting and caching job profile - {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}");

                await GetJobProfileFromContentAPIAndStoreInCache(jobProfileSummary);
            }

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Iterating job profile summary list completed");
        }

        internal async Task GetJobProfileFromContentAPIAndStoreInCache(IGraphSummaryItem jobProfileSummary)
        {
            var contentApiJobProfile = await GetJobProfileFromContentAPI(jobProfileSummary);
            var cacheJobProfile = Mapper.Map<JobProfileCached>(contentApiJobProfile);
            await StoreJobProfileInCache(jobProfileSummary, cacheJobProfile);
        }

        internal async Task<ContentApiJobProfile> GetJobProfileFromContentAPI(IGraphSummaryItem jobProfileSummary)
        {
            _ = jobProfileSummary ?? throw new ArgumentNullException(nameof(jobProfileSummary));

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Getting job profile - {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}");

            var contentApiJobProfile = await GraphContent.GetContentItem<ContentApiJobProfile, ContentApiBranchElement>(jobProfileSummary.Uri);
            if (contentApiJobProfile == null || contentApiJobProfile.IsFaultedState())
            {
                var errorMessage = $"{Utils.LoggerMethodNamePrefix()} *** JOB PROFILE NOT FOUND ON CONTENT API *** - {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}";
                Logger.LogWarning(errorMessage);

                throw new ArgumentException(errorMessage);
            }

            contentApiJobProfile = PopulateJobProfileSections(contentApiJobProfile);

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Returning job profile - {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}");

            return contentApiJobProfile;
        }

        internal async Task StoreJobProfileInCache(IGraphSummaryItem jobProfileSummary, JobProfileCached jobProfileCacheModel)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Updating job profile cache with {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}");

            var result = await _messageService.UpdateAsync(jobProfileCacheModel);
            if (result == HttpStatusCode.NotFound)
            {
                Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Job profile not in cache. Creating cache entry for {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}");

                result = await _messageService.CreateAsync(jobProfileCacheModel);
                if (result != HttpStatusCode.Created)
                {
                    Logger.LogError($"{Utils.LoggerMethodNamePrefix()} *** FAILED TO CREATE JOB PROFILE CACHE ENTRY FOR {jobProfileSummary.CanonicalName} - {jobProfileSummary.Uri}  ERROR STATUS {result}***");
                }
            }
        }

        internal async Task DeleteStaleCacheEntries(
            IReadOnlyCollection<IGraphSummaryItem> graphSummaryItems,
            CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Delete stale cache items started");

            var cachedContentPages = (await _messageService.GetAllCachedCanonicalNamesAsync()).AsSafeReadOnlyList();

            if (cachedContentPages.Any())
            {
                var hashedSummaryList = new HashSet<Uri>(graphSummaryItems.Select(p => p.Uri));
                var staleContentPages = cachedContentPages.Where(p => !hashedSummaryList.Contains(p.Uri)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItems(staleContentPages, stoppingToken);
                }
            }

            Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Delete stale cache items completed");
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
                    Logger.LogWarning($"{Utils.LoggerMethodNamePrefix()} Delete stale cache items cancelled");
                    return;
                }

                Logger.LogInformation($"{Utils.LoggerMethodNamePrefix()} Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await _messageService.DeleteAsync(staleContentPage.Id);

                if (deletionResult != HttpStatusCode.OK)
                {
                    Logger.LogError($"{Utils.LoggerMethodNamePrefix()} Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        internal ContentApiJobProfile PopulateJobProfileSections(ContentApiJobProfile contentApiJobProfile)
        {
            var contentItems = contentApiJobProfile.ContentItems.Flatten(s => s.ContentItems);

            // how to become (root items)
            var moreInfo = new ContentApiHowToBecomeMoreInformation();
            var howToBecome = new ContentApiHowToBecome();

            moreInfo.CareerTips = contentApiJobProfile.HowToBecomeCareerTips;
            moreInfo.ProfessionalBodies = contentApiJobProfile.HowToBecomeProfessionalBodies;
            moreInfo.FurtherInformation = contentApiJobProfile.HowToBecomeFurtherInformation;

            howToBecome.MoreInformation = moreInfo;
            contentApiJobProfile.HowToBecome = howToBecome;

            // what it takes (root items)
            var whatItTakes = new ContentApiWhatItTakes();
            contentApiJobProfile.WhatItTakes = whatItTakes;

            // what you'll do (root items)
            var whatYouWillDo = new ContentApiWhatYouWillDo();
            whatYouWillDo.DayToDayTasks = contentApiJobProfile.WhatYouWillDoDayToDayTasks;
            contentApiJobProfile.WhatYouWillDo = whatYouWillDo;

            // career path (root items)
            var careerPathSegment = new ContentApiCareerPath();
            careerPathSegment.CareerPathAndProgression = contentApiJobProfile.CareerPathAndProgression;
            contentApiJobProfile.CareerPath = careerPathSegment;

            if (!contentItems.Any())
            {
                Logger.LogError($"{Utils.LoggerMethodNamePrefix()} No Content Items for '{contentApiJobProfile.CanonicalName}'");
                return contentApiJobProfile;
            }

            // root element (content stubs)
            contentApiJobProfile.SocCode = GetRawText(contentItems, "SOCCode");
            contentApiJobProfile.RelatedCareers = Make5RelatedCareers(contentItems);

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
            whatItTakes.Restrictions = GetDescriptions(contentItems, "Restriction");
            whatItTakes.OtherRequirements = GetDescriptions(contentItems, "OtherRequirement");

            var witAddition = GetText(contentItems, "DigitalSkillsLevel");
            var skills = GetDescriptions(contentItems, "ONetSkill");
            whatItTakes.Skills = It.Has(witAddition)
                ? skills.Concat(new string[] { witAddition }).AsSafeReadOnlyList()
                : skills.AsSafeReadOnlyList();

            // what you'll do (content items)
            whatYouWillDo.WorkingEnvironment = GetDescriptions(contentItems, "WorkingEnvironment");
            whatYouWillDo.WorkingLocation = GetDescriptions(contentItems, "WorkingLocation");
            whatYouWillDo.WorkingUniform = GetDescriptions(contentItems, "WorkingUniform");

            // career path (content items)
            // careerPathSegment.ApprenticeshipStandard = GetDescriptions(contentItems, "ApprenticeshipStandard")
            return contentApiJobProfile;
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

            route.Topic = GetRouteTopic(contentTopic);
            route.RelevantSubjects = content.RelevantSubjects;
            route.FurtherInformation = content.FurtherInformation;
            route.RequirementsAndReading = item;

            // item.Preface = GetDescription(branches, "RequirementsPrefix")
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

            route.Topic = GetRouteTopic(contentTopic);
            route.Descriptions = GetDescriptions(branches, $"{contentTopic}Route");

            return route;
        }

        internal string GetRouteTopic(string candidate) =>
            candidate switch
            {
                "Direct" => "Direct Application",
                "Volunteering" => "Volunteering and work experience",
                "Other" => "Specialist training",
                _ => candidate,
            };
    }
}
