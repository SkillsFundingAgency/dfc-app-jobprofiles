using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Extensions;
using DFC.App.JobProfile.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.CacheContentService
{
    public class JobProfileCacheLoader :
        ILoadJobProfileContent
    {
        private readonly ILogger<JobProfileCacheLoader> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly IEventMessageService<JobProfileCached> _messageService;
        private readonly IProvideGraphContent _graphContent;
        private readonly IContentCacheService _otherCacheThingy;
         
        public JobProfileCacheLoader(
            ILogger<JobProfileCacheLoader> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<JobProfileCached> eventMessageService,
            IProvideGraphContent cmsApiService,
            IContentCacheService contentCacheService)
        {
            _logger = logger;
            _mapper = mapper;
            _messageService = eventMessageService;
            _graphContent = cmsApiService;
            _otherCacheThingy = contentCacheService;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Reload cache started");

                var summaryList = await GetSummaryListAsync().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Reload cache cancelled");

                    return;
                }

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryListAsync(summaryList, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Reload cache cancelled");

                        return;
                    }

                    await DeleteStaleCacheEntriesAsync(summaryList, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in cache reload");
            }
        }

        public async Task<IReadOnlyCollection<SummaryItemModel>> GetSummaryListAsync()
        {
            _logger.LogInformation("Get summary list");

            var summaryList = await _graphContent.GetSummaryItems<SummaryItemModel>().ConfigureAwait(false);

            _logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync(
            IReadOnlyCollection<SummaryItemModel> summaryList,
            CancellationToken stoppingToken)
        {
            _logger.LogInformation("Process summary list started");

            _otherCacheThingy.Clear();

            foreach (var item in summaryList.OrderBy(o => o.Published))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItemAsync(item, stoppingToken).ConfigureAwait(false);
            }

            _logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(SummaryItemModel item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            try
            {
                _logger.LogInformation($"Get details for {item.CanonicalName} - {item.Uri}");

                var apiDataModel = await _graphContent
                    .GetComposedItem<ContentApiRootElement, ContentApiBranchElement>(item.Uri)
                    .ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    _logger.LogWarning($"No details returned from {item.CanonicalName} - {item.Uri}");

                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Process item get and save cancelled");

                    return;
                }

                OrganiseSegments(ref apiDataModel);
                var contentPageModel = _mapper.Map<JobProfileCached>(apiDataModel);

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

                var contentItemIds = contentPageModel.AllContentItemIds.ToList();
                _otherCacheThingy.AddOrReplace(contentPageModel.Id, contentItemIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in get and save for {item.CanonicalName} - {item.Uri}");
            }
        }

        public async Task DeleteStaleCacheEntriesAsync(
            IReadOnlyCollection<SummaryItemModel> summaryList,
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
                    await DeleteStaleItemsAsync(staleContentPages, stoppingToken).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Delete stale cache items completed");
        }

        public async Task DeleteStaleItemsAsync(
            IReadOnlyCollection<JobProfileCached> staleItems,
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

        public bool TryValidateModel(JobProfileCached contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    _logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Uri}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }

        private void OrganiseSegments(ref ContentApiRootElement apiDataModel)
        {
            //What You'll Do
            var whatYoullDoModel = new JobProfileCachedWhatYoullDo();
            whatYoullDoModel.DaytoDayTasks = apiDataModel.ContentItems.Where(x => x.ContentType == "DayToDayTask").ToList();
            whatYoullDoModel.WorkingEnvironment = apiDataModel.ContentItems.Where(x => x.ContentType == "WorkingEnvironment").ToList();
            whatYoullDoModel.WorkingLocation = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "WorkingLocation");
            whatYoullDoModel.WorkingUniform = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "WorkingUniform");
            apiDataModel.WhatYoullDoSegment = whatYoullDoModel;

            //Career Path
            var careerPathSegment = new JobProfileCachedCareerPath();
            careerPathSegment.ApprecticeshipStandard = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipStandard").ToList();
            careerPathSegment.CareerPathAndProgression = apiDataModel.CareerPathAndProgression;
            apiDataModel.CareerPathSegment = careerPathSegment;

            //How to Become
            var howToBecomeSegment = new JobProfileCachedHowToBecome();
            howToBecomeSegment.DirectRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "DirectRoute");
            howToBecomeSegment.ApprenticeshipRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "ApprenticeshipRoute");
            howToBecomeSegment.CollegeRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "CollegeRoute");
            howToBecomeSegment.OtherRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "OtherRoute");
            howToBecomeSegment.Registration = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "Registration");
            howToBecomeSegment.UniversityRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "UniversityRoute");
            howToBecomeSegment.VolunteeringRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "VolunteeringRoute");
            howToBecomeSegment.WorkRoute = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "WorkRoute");
            howToBecomeSegment.Title = apiDataModel.Title;
            howToBecomeSegment.HtbBodies = apiDataModel.HtbBodies;
            howToBecomeSegment.HtbFurtherInformation = apiDataModel.HtbFurtherInformation;
            apiDataModel.HowToBecomeSegment = howToBecomeSegment;

            //What it Takes
            var whatItTakes = new JobProfileCachedWhatItTakes();
            whatItTakes.Restrictions = apiDataModel.ContentItems.Where(x => x.ContentType == "Restriction").ToList();
            whatItTakes.OtherRequirement = apiDataModel.ContentItems.Where(x => x.ContentType == "OtherRequirement").ToList();
            whatItTakes.Occupation = apiDataModel.ContentItems.SingleOrDefault(x => x.ContentType == "occupation");
            apiDataModel.WhatItTakesSegment = whatItTakes;

            apiDataModel.AllContentItemIds = apiDataModel.ContentItems.Flatten(s => s.ContentItems).Select(s => s.ItemID).ToList();
        }
    }
}
