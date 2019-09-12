using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class SegmentService : ISegmentService
    {
        private readonly ILogger<SegmentService> logger;
        private readonly ICareerPathSegmentService careerPathSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IHowToBecomeSegmentService howToBecomeSegmentService;
        private readonly IOverviewBannerSegmentService overviewBannerSegmentService;
        private readonly IRelatedCareersSegmentService relatedCareersSegmentService;
        private readonly IWhatItTakesSegmentService whatItTakesSegmentService;
        private readonly IWhatYouWillDoSegmentService whatYouWillDoSegmentService;

        public SegmentService(
            ILogger<SegmentService> logger,
            ICareerPathSegmentService careerPathSegmentService,
            ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService,
            IHowToBecomeSegmentService howToBecomeSegmentService,
            IOverviewBannerSegmentService overviewBannerSegmentService,
            IRelatedCareersSegmentService relatedCareersSegmentService,
            IWhatItTakesSegmentService whatItTakesSegmentService,
            IWhatYouWillDoSegmentService whatYouWillDoSegmentService)
        {
            this.logger = logger;
            this.careerPathSegmentService = careerPathSegmentService;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.howToBecomeSegmentService = howToBecomeSegmentService;
            this.overviewBannerSegmentService = overviewBannerSegmentService;
            this.relatedCareersSegmentService = relatedCareersSegmentService;
            this.whatItTakesSegmentService = whatItTakesSegmentService;
            this.whatYouWillDoSegmentService = whatYouWillDoSegmentService;
        }

        public CreateOrUpdateJobProfileModel CreateOrUpdateJobProfileModel { get; set; }

        public JobProfileModel JobProfileModel { get; set; }

        public async Task LoadAsync()
        {
            logger.LogInformation($"{nameof(LoadAsync)}: Loading segments for {CreateOrUpdateJobProfileModel.CanonicalName}");

            var tasks = new List<Task>();

            Task<CareerPathSegmentModel> careerPathSegmnentDataTask = null;
            Task<CurrentOpportunitiesSegmentModel> currentOpportunitiesSegmnentDataTask = null;
            Task<HowToBecomeSegmentModel> howToBecomeSegmnentDataTask = null;
            Task<OverviewBannerSegmentModel> overviewBannerSegmnentDataTask = null;
            Task<RelatedCareersSegmentModel> relatedCareersSegmnentDataTask = null;
            Task<WhatItTakesSegmentModel> whatItTakesSegmnentDataTask = null;
            Task<WhatYouWillDoSegmentModel> whatYouWillDoSegmnentDataTask = null;

            Task<string> careerPathSegmnentMarkupTask = null;
            Task<string> currentOpportunitiesSegmnentMarkupTask = null;
            Task<string> howToBecomeSegmnentMarkupTask = null;
            Task<string> overviewBannerSegmnentMarkupTask = null;
            Task<string> relatedCareersSegmnentMarkupTask = null;
            Task<string> whatItTakesSegmnentMarkupTask = null;
            Task<string> whatYouWillDoSegmnentMarkupTask = null;

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshCareerPathSegment)
            {
                careerPathSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                careerPathSegmnentDataTask = careerPathSegmentService.LoadDataAsync();
                tasks.Add(careerPathSegmnentDataTask);

                careerPathSegmnentMarkupTask = careerPathSegmentService.LoadMarkupAsync();
                tasks.Add(careerPathSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                currentOpportunitiesSegmnentDataTask = currentOpportunitiesSegmentService.LoadDataAsync();
                tasks.Add(currentOpportunitiesSegmnentDataTask);

                currentOpportunitiesSegmnentMarkupTask = currentOpportunitiesSegmentService.LoadMarkupAsync();
                tasks.Add(currentOpportunitiesSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshHowToBecomeSegment)
            {
                howToBecomeSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                howToBecomeSegmnentDataTask = howToBecomeSegmentService.LoadDataAsync();
                tasks.Add(howToBecomeSegmnentDataTask);

                howToBecomeSegmnentMarkupTask = howToBecomeSegmentService.LoadMarkupAsync();
                tasks.Add(howToBecomeSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshOverviewBannerSegment)
            {
                overviewBannerSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                overviewBannerSegmnentDataTask = overviewBannerSegmentService.LoadDataAsync();
                tasks.Add(overviewBannerSegmnentDataTask);

                overviewBannerSegmnentMarkupTask = overviewBannerSegmentService.LoadMarkupAsync();
                tasks.Add(overviewBannerSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshRelatedCareersSegment)
            {
                relatedCareersSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                relatedCareersSegmnentDataTask = relatedCareersSegmentService.LoadDataAsync();
                tasks.Add(relatedCareersSegmnentDataTask);

                relatedCareersSegmnentMarkupTask = relatedCareersSegmentService.LoadMarkupAsync();
                tasks.Add(relatedCareersSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshWhatItTakesSegment)
            {
                whatItTakesSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                whatItTakesSegmnentDataTask = whatItTakesSegmentService.LoadDataAsync();
                tasks.Add(whatItTakesSegmnentDataTask);

                whatItTakesSegmnentMarkupTask = whatItTakesSegmentService.LoadMarkupAsync();
                tasks.Add(whatItTakesSegmnentMarkupTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshWhatYouWillDoSegment)
            {
                whatYouWillDoSegmentService.CanonicalName = CreateOrUpdateJobProfileModel.CanonicalName;

                whatYouWillDoSegmnentDataTask = whatYouWillDoSegmentService.LoadDataAsync();
                tasks.Add(whatYouWillDoSegmnentDataTask);

                whatYouWillDoSegmnentMarkupTask = whatYouWillDoSegmentService.LoadMarkupAsync();
                tasks.Add(whatYouWillDoSegmnentMarkupTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            JobProfileModel.Data.CareerPath = GetDataResult(careerPathSegmnentDataTask);
            JobProfileModel.Data.CurrentOpportunities = GetDataResult(currentOpportunitiesSegmnentDataTask);
            JobProfileModel.Data.HowToBecome = GetDataResult(howToBecomeSegmnentDataTask);
            JobProfileModel.Data.OverviewBanner = GetDataResult(overviewBannerSegmnentDataTask);
            JobProfileModel.Data.RelatedCareers = GetDataResult(relatedCareersSegmnentDataTask);
            JobProfileModel.Data.WhatItTakes = GetDataResult(whatItTakesSegmnentDataTask);
            JobProfileModel.Data.WhatYouWillDo = GetDataResult(whatYouWillDoSegmnentDataTask);

            JobProfileModel.Markup.CareerPath = GetMarkupResult(careerPathSegmnentMarkupTask, careerPathSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.CurrentOpportunities = GetMarkupResult(currentOpportunitiesSegmnentMarkupTask, currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.HowToBecome = GetMarkupResult(howToBecomeSegmnentMarkupTask, howToBecomeSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.OverviewBanner = GetMarkupResult(overviewBannerSegmnentMarkupTask, overviewBannerSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.RelatedCareers = GetMarkupResult(relatedCareersSegmnentMarkupTask, relatedCareersSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.WhatItTakes = GetMarkupResult(whatItTakesSegmnentMarkupTask, whatItTakesSegmentService.SegmentClientOptions.OfflineHtml);
            JobProfileModel.Markup.WhatYouWillDo = GetMarkupResult(whatYouWillDoSegmnentMarkupTask, whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml);

            JobProfileModel.Updated = DateTime.UtcNow;

            logger.LogInformation($"{nameof(LoadAsync)}: Loaded segments for {CreateOrUpdateJobProfileModel.CanonicalName}");
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            var tasks = new List<Task<IList<HealthCheckItem>>>
            {
                careerPathSegmentService.HealthCheckAsync(),
                currentOpportunitiesSegmentService.HealthCheckAsync(),
                howToBecomeSegmentService.HealthCheckAsync(),
                overviewBannerSegmentService.HealthCheckAsync(),
                relatedCareersSegmentService.HealthCheckAsync(),
                whatItTakesSegmentService.HealthCheckAsync(),
                whatYouWillDoSegmentService.HealthCheckAsync(),
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var result = new List<HealthCheckItem>();

            tasks.ForEach(f => result.AddRange(GetHealthResults(f)));

            return result;
        }

        private TModel GetDataResult<TModel>(Task<TModel> task)
            where TModel : BaseSegmentModel, new()
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    return task.Result;
                }
            }

            var noResultsModel = new TModel
            {
                Updated = DateTime.UtcNow,
            };

            return noResultsModel;
        }

        private string GetMarkupResult(Task<string> task, string offlineHtml)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    return task.Result;
                }
            }

            return offlineHtml;
        }

        private IList<HealthCheckItem> GetHealthResults(Task<IList<HealthCheckItem>> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    return task.Result;
                }
            }

            return null;
        }
    }
}
