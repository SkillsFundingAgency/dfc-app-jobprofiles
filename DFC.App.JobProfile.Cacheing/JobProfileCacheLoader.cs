// TODO: content cache service (other cache thingy) don't understand what this does...
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
using DFC.App.JobProfile.Cacheing.Models;
using DFC.App.JobProfile.ContentAPI.Models;
using DFC.App.JobProfile.ContentAPI.Services;
using DFC.App.JobProfile.EventProcessing;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Cacheing
{
    public class JobProfileCacheLoader<TModel> :
        ILoadJobProfileContent
        where TModel : class, IContentPageModel, IResourceLocatable
    {
        private readonly ILogger<JobProfileCacheLoader<TModel>> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IEventMessageService<TModel> _messageService;
        private readonly IProvideGraphContent _graphContent;
        //private readonly IContentCacheService _otherCacheThingy;

        public JobProfileCacheLoader(
            ILogger<JobProfileCacheLoader<TModel>> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<TModel> eventMessageService,
            IProvideGraphContent cmsApiService)
        //,IContentCacheService otherCacheThingy)
        {
            _logger = logger;
            _mapper = mapper;
            _messageService = eventMessageService;
            _graphContent = cmsApiService;
            //_otherCacheThingy = otherCacheThingy;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Reload cache started");

                var summaryList = await GetSummaryList().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Reload cache cancelled");

                    return;
                }

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryList(summaryList, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Reload cache cancelled");

                        return;
                    }

                    await DeleteStaleCacheEntries(summaryList, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cache reload");
            }
        }

        internal async Task<IReadOnlyCollection<GraphSummaryItem>> GetSummaryList()
        {
            _logger.LogInformation("Get summary list");

            var summaryList = await _graphContent.GetSummaryItems<GraphSummaryItem>().ConfigureAwait(false);

            _logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        internal async Task ProcessSummaryList(
            IReadOnlyCollection<GraphSummaryItem> summaryList,
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Process summary list started");

            //_otherCacheThingy.Clear();

            foreach (var item in summaryList.OrderBy(o => o.Published))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItem(item, stoppingToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Process summary list completed");
        }

        internal async Task GetAndSaveItem(GraphSummaryItem item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            try
            {
                _logger.LogInformation($"Get details for {item.CanonicalName} - {item.Uri}");

                var apiDataModel = await _graphContent
                    .GetComposedItem<ContentApiRootElement, ContentApiBranchElement>(item.Uri)
                    .ConfigureAwait(false);

                if (apiDataModel == null || apiDataModel.IsFaultedState())
                {
                    _logger.LogWarning($"No details returned from {item.CanonicalName} - {item.Uri}");

                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Process item get and save cancelled");

                    return;
                }

                apiDataModel = OrganiseSegments(apiDataModel);
                var contentPageModel = _mapper.Map<TModel>(apiDataModel);

                _logger.LogInformation($"Updating cache with {item.CanonicalName} - {item.Uri}");

                var result = await _messageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"Does not exist, creating cache with {item.CanonicalName} - {item.Uri}");

                    result = await _messageService.CreateAsync(contentPageModel).ConfigureAwait(false);

                    if (result == HttpStatusCode.Created)
                    {
                        _logger.LogInformation($"Created cache with {item.CanonicalName} - {item.Uri}");
                    }
                    else
                    {
                        _logger.LogError($"Cache create error status {result} from {item.CanonicalName} - {item.Uri}");
                    }
                }
                else
                {
                    _logger.LogInformation($"Updated cache with {item.CanonicalName} - {item.Uri}");
                }

                //var contentItemIds = contentPageModel.AllContentItemIds.ToList();
                //_otherCacheThingy.AddOrReplace(contentPageModel.Id, contentItemIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in get and save for {item.CanonicalName} - {item.Uri}");
            }
        }

        internal async Task DeleteStaleCacheEntries(
            IReadOnlyCollection<GraphSummaryItem> summaryList,
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = await _messageService.GetAllCachedCanonicalNamesAsync().ConfigureAwait(false);

            if (cachedContentPages != null && cachedContentPages.Any())
            {
                var hashedSummaryList = new HashSet<Uri>(summaryList.Select(p => p.Uri));
                var staleContentPages = cachedContentPages.Where(p => !hashedSummaryList.Contains(p.Uri)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItems(staleContentPages, stoppingToken).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Delete stale cache items completed");
        }

        internal async Task DeleteStaleItems(
            IReadOnlyCollection<TModel> staleItems,
            CancellationToken stoppingToken)
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Delete stale cache items cancelled");

                    return;
                }

                _logger.LogInformation($"Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await _messageService.DeleteAsync(staleContentPage.Id).ConfigureAwait(false);

                if (deletionResult == HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Deleted stale cache item {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
                else
                {
                    _logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        //internal bool TryValidateModel(TModel contentModel)
        //{
        //    _ = contentModel ?? throw new ArgumentNullException(nameof(contentModel));

        //    var validationContext = new ValidationContext(contentModel, null, null);
        //    var validationResults = new List<ValidationResult>();
        //    var isValid = Validator.TryValidateObject(contentModel, validationContext, validationResults, true);

        //    if (!isValid && validationResults.Any())
        //    {
        //        foreach (var validationResult in validationResults)
        //        {
        //            _logger.LogError($"Error validating {contentModel.CanonicalName} - {contentModel.Uri}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
        //        }
        //    }

        //    return isValid;
        //}

        internal ContentApiRootElement OrganiseSegments(ContentApiRootElement apiDataModel)
        {
            var contentItems = apiDataModel.ContentItems.Flatten(s => s.ContentItems);

            if (!contentItems.Any())
            {
                _logger.LogError($"No Content Items for '{apiDataModel.CanonicalName}'");
                return apiDataModel;
            }

            //What You'll Do
            var whatYoullDoModel = new ContentApiWhatYoullDo();
            whatYoullDoModel.DaytoDayTasks = GetRawTexts(contentItems, "DayToDayTask");
            whatYoullDoModel.WorkingEnvironment = GetDescriptions(contentItems, "WorkingEnvironment");
            whatYoullDoModel.WorkingLocation = GetDescriptions(contentItems, "WorkingLocation");
            whatYoullDoModel.WorkingUniform = GetDescriptions(contentItems, "WorkingUniform");
            apiDataModel.WhatYoullDoSegment = whatYoullDoModel;

            //Career Path
            var careerPathSegment = new ContentApiCareerPath();
            //careerPathSegment.ApprenticeshipStandard = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipStandard").ToList();
            careerPathSegment.CareerPathAndProgression = apiDataModel.CareerPathAndProgression;
            apiDataModel.CareerPathSegment = careerPathSegment;

            //How to Become
            var moreInfo = new ContentApiHowToBecomeMoreInformation();
            var howToBecomeSegment = new ContentApiHowToBecome();
            howToBecomeSegment.Title = apiDataModel.Title;

            howToBecomeSegment.DirectRoute = GetDescription(contentItems, "DirectRoute");
            howToBecomeSegment.OtherRoute = GetDescription(contentItems, "OtherRoute");
            howToBecomeSegment.Registration = GetDescription(contentItems, "Registration");
            howToBecomeSegment.VolunteeringRoute = GetDescription(contentItems, "VolunteeringRoute");
            howToBecomeSegment.WorkRoute = GetDescription(contentItems, "WorkRoute");

            howToBecomeSegment.ApprenticeshipRoute = GetRoute(contentItems, "Apprenticeship");
            howToBecomeSegment.CollegeRoute = GetRoute(contentItems, "College");
            howToBecomeSegment.UniversityRoute = GetRoute(contentItems, "University");

            moreInfo.ProfessionalBodies = apiDataModel.HtbBodies;
            moreInfo.FurtherInformation = apiDataModel.HtbFurtherInformation;
            moreInfo.CareerTips = apiDataModel.HtbCareerTips;
            howToBecomeSegment.MoreInformation = moreInfo;
            apiDataModel.HowToBecomeSegment = howToBecomeSegment;

            //What it Takes
            var whatItTakes = new ContentApiWhatItTakes();
            //whatItTakes.Restrictions = apiDataModel.ContentItems.Where(x => x.ContentType == "Restriction").ToList();
            //whatItTakes.OtherRequirement = apiDataModel.ContentItems.Where(x => x.ContentType == "OtherRequirement").ToList();
            whatItTakes.Skills = GetDescriptions(contentItems, "ONetSkill");
            apiDataModel.WhatItTakesSegment = whatItTakes;

            return apiDataModel;
        }

        internal IReadOnlyCollection<string> GetDescriptions(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            return branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => x.Description)
                .ToList();
        }

        internal IReadOnlyCollection<string> GetRawTexts(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            return branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => x.Title)
                .ToList();
        }

        internal IReadOnlyCollection<ApiAnchor> GetReadingLinks(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            return branches
                .Where(x => x.ContentType == contentType)
                .OrderBy(x => x.Ordinal)
                .Select(x => new ApiAnchor { Text = x.Text, Link = x.Link })
                .ToList();
        }

        internal string GetDescription(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            return branches
                .FirstOrDefault(x => x.ContentType == contentType)?
                .Description;
        }

        internal string GetRawText(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            return branches
                .FirstOrDefault(x => x.ContentType == contentType)?
                .Title;
        }

        internal ApiEducationalRoute GetRoute(IEnumerable<ContentApiBranchElement> branches, string contentType)
        {
            var content = branches.FirstOrDefault(x => x.ContentType == $"{contentType}Route");

            if(content == null)
            {
                return null;
            }

            var route = new ApiEducationalRoute();
            var item = new ApiEducationalRouteItem();

            route.MoreInformation = item;
            route.FurtherInformation = content.FurtherInformation;
            route.RelevantSubjects = content.RelevantSubjects;

            item.Preamble = GetDescription(branches, "RequirementsPrefix");
            item.Requirements = GetDescriptions(branches, $"{contentType}Requirement");
            item.FurtherReading = GetReadingLinks(branches, $"{contentType}Link");

            return route;
        }
    }
}
