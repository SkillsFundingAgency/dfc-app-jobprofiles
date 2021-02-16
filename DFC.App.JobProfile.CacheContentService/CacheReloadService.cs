using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Extensions;
using DFC.App.JobProfile.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
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
    public class CacheReloadService : ICacheReloadService
    {
        private readonly ILogger<CacheReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<ContentPageModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IContentCacheService contentCacheService;
         
        public CacheReloadService(
            ILogger<CacheReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<ContentPageModel> eventMessageService,
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
                logger.LogInformation("Reload cache started");

                var summaryList = await GetSummaryListAsync().ConfigureAwait(false);

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload cache cancelled");

                    return;
                }

                if (summaryList != null && summaryList.Any())
                {
                    await ProcessSummaryListAsync(summaryList, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("Reload cache cancelled");

                        return;
                    }

                    await DeleteStaleCacheEntriesAsync(summaryList, stoppingToken).ConfigureAwait(false);
                }

                logger.LogInformation("Reload cache completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in cache reload");
            }
        }

        public async Task<IList<JobProfileItemModel>> GetSummaryListAsync()
        {
            logger.LogInformation("Get summary list");

            var summaryList = await cmsApiService.GetSummaryAsync<JobProfileItemModel>().ConfigureAwait(false);

            logger.LogInformation("Get summary list completed");

            return summaryList;
        }

        public async Task ProcessSummaryListAsync(IList<JobProfileItemModel>? summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            contentCacheService.Clear();

            foreach (var item in summaryList.OrderBy(o => o.Published))
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process summary list cancelled");

                    return;
                }

                await GetAndSaveItemAsync(item, stoppingToken).ConfigureAwait(false);
            }

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(JobProfileItemModel item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            try
            {
                logger.LogInformation($"Get details for {item.Title} - {item.Url}");

                var apiDataModel = await cmsApiService.GetItemAsync<JobProfileApiDataModel, JobProfileApiContentItemModel>(item.Url).ConfigureAwait(false);

                if (apiDataModel == null)
                {
                    logger.LogWarning($"No details returned from {item.Title} - {item.Url}");

                    return;
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Process item get and save cancelled");

                    return;
                }

                OrganiseSegments(ref apiDataModel);
                var contentPageModel = mapper.Map<ContentPageModel>(apiDataModel);

                logger.LogInformation($"Updating cache with {item.Title} - {item.Url}");

                var result = await eventMessageService.UpdateAsync(contentPageModel).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    logger.LogInformation($"Does not exist, creating cache with {item.Title} - {item.Url}");

                    result = await eventMessageService.CreateAsync(contentPageModel).ConfigureAwait(false);

                    if (result == HttpStatusCode.Created)
                    {
                        logger.LogInformation($"Created cache with {item.Title} - {item.Url}");
                    }
                    else
                    {
                        logger.LogError($"Cache create error status {result} from {item.Title} - {item.Url}");
                    }
                }
                else
                {
                    logger.LogInformation($"Updated cache with {item.Title} - {item.Url}");
                }

                var contentItemIds = contentPageModel.AllContentItemIds;
                contentCacheService.AddOrReplace(contentPageModel.Id, contentItemIds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in get and save for {item.Title} - {item.Url}");
            }
        }

        public async Task DeleteStaleCacheEntriesAsync(IList<JobProfileItemModel> summaryList, CancellationToken stoppingToken)
        {
            logger.LogInformation("Delete stale cache items started");

            var cachedContentPages = await eventMessageService.GetAllCachedCanonicalNamesAsync().ConfigureAwait(false);

            if (cachedContentPages != null && cachedContentPages.Any())
            {
                var hashedSummaryList = new HashSet<Uri>(summaryList.Select(p => p.Url!));
                var staleContentPages = cachedContentPages.Where(p => !hashedSummaryList.Contains(p.Url!)).ToList();

                if (staleContentPages.Any())
                {
                    await DeleteStaleItemsAsync(staleContentPages, stoppingToken).ConfigureAwait(false);
                }
            }

            logger.LogInformation("Delete stale cache items completed");
        }

        public async Task DeleteStaleItemsAsync(List<Data.Models.ContentPageModel> staleItems, CancellationToken stoppingToken)
        {
            _ = staleItems ?? throw new ArgumentNullException(nameof(staleItems));

            foreach (var staleContentPage in staleItems)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Delete stale cache items cancelled");

                    return;
                }

                logger.LogInformation($"Deleting cache with {staleContentPage.CanonicalName} - {staleContentPage.Id}");

                var deletionResult = await eventMessageService.DeleteAsync(staleContentPage.Id).ConfigureAwait(false);

                if (deletionResult == HttpStatusCode.OK)
                {
                    logger.LogInformation($"Deleted stale cache item {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
                else
                {
                    logger.LogError($"Cache delete error status {deletionResult} from {staleContentPage.CanonicalName} - {staleContentPage.Id}");
                }
            }
        }

        public bool TryValidateModel(Data.Models.ContentPageModel contentPageModel)
        {
            _ = contentPageModel ?? throw new ArgumentNullException(nameof(contentPageModel));

            var validationContext = new ValidationContext(contentPageModel, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(contentPageModel, validationContext, validationResults, true);

            if (!isValid && validationResults.Any())
            {
                foreach (var validationResult in validationResults)
                {
                    logger.LogError($"Error validating {contentPageModel.CanonicalName} - {contentPageModel.Url}: {string.Join(",", validationResult.MemberNames)} - {validationResult.ErrorMessage}");
                }
            }

            return isValid;
        }

        private void OrganiseSegments(ref JobProfileApiDataModel apiDataModel)
        {
            //What You'll Do
            var whatYoullDoModel = new JobProfileWhatYoullDoModel();
            whatYoullDoModel.DaytoDayTasks = apiDataModel.ContentItems.Where(x => x.ContentType == "DayToDayTask").ToList();
            whatYoullDoModel.WorkingEnvironment = apiDataModel.ContentItems.Where(x => x.ContentType == "WorkingEnvironment").ToList();
            whatYoullDoModel.WorkingLocation = apiDataModel.ContentItems.Where(x => x.ContentType == "WorkingLocation").SingleOrDefault();
            whatYoullDoModel.WorkingUniform = apiDataModel.ContentItems.Where(x => x.ContentType == "WorkingUniform").SingleOrDefault();
            apiDataModel.WhatYoullDoSegment = whatYoullDoModel;

            //Career Path
            var careerPathSegment = new JobProfileCareerPathModel();
            careerPathSegment.ApprecticeshipStandard = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipStandard").ToList();
            careerPathSegment.CareerPathAndProgression = apiDataModel.CareerPathAndProgression;
            apiDataModel.CareerPathSegment = careerPathSegment;

            //How to Become
            var howToBecomeSegment = new JobProfileHowToBecomeModel();
            howToBecomeSegment.DirectRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "DirectRoute").SingleOrDefault();
            howToBecomeSegment.ApprenticeshipRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "ApprenticeshipRoute").SingleOrDefault();
            howToBecomeSegment.CollegeRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "CollegeRoute").SingleOrDefault();
            howToBecomeSegment.OtherRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "OtherRoute").SingleOrDefault();
            howToBecomeSegment.Registration = apiDataModel.ContentItems.Where(x => x.ContentType == "Registration").SingleOrDefault();
            howToBecomeSegment.UniversityRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "UniversityRoute").SingleOrDefault();
            howToBecomeSegment.VolunteeringRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "VolunteeringRoute").SingleOrDefault();
            howToBecomeSegment.WorkRoute = apiDataModel.ContentItems.Where(x => x.ContentType == "WorkRoute").SingleOrDefault();
            howToBecomeSegment.Title = apiDataModel.Title;
            howToBecomeSegment.HtbBodies = apiDataModel.HtbBodies;
            howToBecomeSegment.HtbFurtherInformation = apiDataModel.HtbFurtherInformation;
            apiDataModel.HowToBecomeSegment = howToBecomeSegment;

            //What it Takes
            var whatItTakes = new JobProfileWhatItTakesModel();
            whatItTakes.Restrictions = apiDataModel.ContentItems.Where(x => x.ContentType == "Restriction").ToList();
            whatItTakes.OtherRequirement = apiDataModel.ContentItems.Where(x => x.ContentType == "OtherRequirement").ToList();
            whatItTakes.Occupation = apiDataModel.ContentItems.Where(x => x.ContentType == "occupation").SingleOrDefault();
            apiDataModel.WhatItTakesSegment = whatItTakes;

            apiDataModel.AllContentItemIds = apiDataModel.ContentItems.Flatten(s => s.ContentItems).Where(w => w.ItemId != null).Select(s => s.ItemId!.Value).ToList();
        }
    }
}
