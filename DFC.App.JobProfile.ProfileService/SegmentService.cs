using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileSkillModels;
using DFC.App.JobProfile.Data.Models.Segments.JobProfileTasksModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
using DFC.App.JobProfile.ProfileService.Utilities;
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

        public RefreshJobProfileSegmentModel RefreshJobProfileSegmentModel { get; set; }

        public JobProfileModel JobProfileModel { get; set; }

        public Uri RequestBaseAddress { get; set; }

        public async Task LoadAsync()
        {
            logger.LogInformation($"{nameof(LoadAsync)}: Loading segments for {RefreshJobProfileSegmentModel.CanonicalName}");

            var tasks = new List<Task>();

            bool refreshAllSegments = string.IsNullOrWhiteSpace(RefreshJobProfileSegmentModel.Segment);
            bool refreshCareerPathSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == CareerPathSegmentDataModel.SegmentName;
            bool refreshCurrentOpportunitiesSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == CurrentOpportunitiesSegmentDataModel.SegmentName;
            bool refreshHowToBecomeSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == HowToBecomeSegmentDataModel.SegmentName;
            bool refreshOverviewBannerSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == OverviewBannerSegmentDataModel.SegmentName;
            bool refreshRelatedCareersSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == RelatedCareersSegmentDataModel.SegmentName;
            bool refreshWhatItTakesSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == JobProfileSkillSegmentDataModel.SegmentName;
            bool refreshWhatYouWillDoSegment = refreshAllSegments || RefreshJobProfileSegmentModel.Segment == JobProfileTasksSegmentDataModel.SegmentName;

            Task<CareerPathSegmentDataModel> careerPathSegmnentDataTask = null;
            Task<CurrentOpportunitiesSegmentDataModel> currentOpportunitiesSegmnentDataTask = null;
            Task<HowToBecomeSegmentDataModel> howToBecomeSegmnentDataTask = null;
            Task<OverviewBannerSegmentDataModel> overviewBannerSegmnentDataTask = null;
            Task<RelatedCareersSegmentDataModel> relatedCareersSegmnentDataTask = null;
            Task<JobProfileSkillSegmentDataModel> whatItTakesSegmnentDataTask = null;
            Task<JobProfileTasksSegmentDataModel> whatYouWillDoSegmnentDataTask = null;

            Task<string> careerPathSegmnentMarkupTask = null;
            Task<string> currentOpportunitiesSegmnentMarkupTask = null;
            Task<string> howToBecomeSegmnentMarkupTask = null;
            Task<string> overviewBannerSegmnentMarkupTask = null;
            Task<string> relatedCareersSegmnentMarkupTask = null;
            Task<string> whatItTakesSegmnentMarkupTask = null;
            Task<string> whatYouWillDoSegmnentMarkupTask = null;

            if (refreshCareerPathSegment)
            {
                careerPathSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                careerPathSegmnentDataTask = careerPathSegmentService.LoadDataAsync();
                tasks.Add(careerPathSegmnentDataTask);

                careerPathSegmnentMarkupTask = careerPathSegmentService.LoadMarkupAsync();
                tasks.Add(careerPathSegmnentMarkupTask);
            }

            if (refreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                currentOpportunitiesSegmnentDataTask = currentOpportunitiesSegmentService.LoadDataAsync();
                tasks.Add(currentOpportunitiesSegmnentDataTask);

                currentOpportunitiesSegmnentMarkupTask = currentOpportunitiesSegmentService.LoadMarkupAsync();
                tasks.Add(currentOpportunitiesSegmnentMarkupTask);
            }

            if (refreshHowToBecomeSegment)
            {
                howToBecomeSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                howToBecomeSegmnentDataTask = howToBecomeSegmentService.LoadDataAsync();
                tasks.Add(howToBecomeSegmnentDataTask);

                howToBecomeSegmnentMarkupTask = howToBecomeSegmentService.LoadMarkupAsync();
                tasks.Add(howToBecomeSegmnentMarkupTask);
            }

            if (refreshOverviewBannerSegment)
            {
                overviewBannerSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                overviewBannerSegmnentDataTask = overviewBannerSegmentService.LoadDataAsync();
                tasks.Add(overviewBannerSegmnentDataTask);

                overviewBannerSegmnentMarkupTask = overviewBannerSegmentService.LoadMarkupAsync();
                tasks.Add(overviewBannerSegmnentMarkupTask);
            }

            if (refreshRelatedCareersSegment)
            {
                relatedCareersSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                relatedCareersSegmnentDataTask = relatedCareersSegmentService.LoadDataAsync();
                tasks.Add(relatedCareersSegmnentDataTask);

                relatedCareersSegmnentMarkupTask = relatedCareersSegmentService.LoadMarkupAsync();
                tasks.Add(relatedCareersSegmnentMarkupTask);
            }

            if (refreshWhatItTakesSegment)
            {
                whatItTakesSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                whatItTakesSegmnentDataTask = whatItTakesSegmentService.LoadDataAsync();
                tasks.Add(whatItTakesSegmnentDataTask);

                whatItTakesSegmnentMarkupTask = whatItTakesSegmentService.LoadMarkupAsync();
                tasks.Add(whatItTakesSegmnentMarkupTask);
            }

            if (refreshWhatYouWillDoSegment)
            {
                whatYouWillDoSegmentService.DocumentId = RefreshJobProfileSegmentModel.DocumentId;

                whatYouWillDoSegmnentDataTask = whatYouWillDoSegmentService.LoadDataAsync();
                tasks.Add(whatYouWillDoSegmnentDataTask);

                whatYouWillDoSegmnentMarkupTask = whatYouWillDoSegmentService.LoadMarkupAsync();
                tasks.Add(whatYouWillDoSegmnentMarkupTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            if (refreshCareerPathSegment)
            {
                JobProfileModel.Data.CareerPath = GetDataResult(careerPathSegmnentDataTask);
                JobProfileModel.Markup.CareerPath = GetMarkupResult(careerPathSegmnentMarkupTask, careerPathSegmentService.SegmentClientOptions);
            }

            if (refreshCurrentOpportunitiesSegment)
            {
                JobProfileModel.Data.CurrentOpportunities = GetDataResult(currentOpportunitiesSegmnentDataTask);
                JobProfileModel.Markup.CurrentOpportunities = GetMarkupResult(currentOpportunitiesSegmnentMarkupTask, currentOpportunitiesSegmentService.SegmentClientOptions);
            }

            if (refreshHowToBecomeSegment)
            {
                JobProfileModel.Data.HowToBecome = GetDataResult(howToBecomeSegmnentDataTask);
                JobProfileModel.Markup.HowToBecome = GetMarkupResult(howToBecomeSegmnentMarkupTask, howToBecomeSegmentService.SegmentClientOptions);
            }

            if (refreshOverviewBannerSegment)
            {
                JobProfileModel.Data.OverviewBanner = GetDataResult(overviewBannerSegmnentDataTask);
                JobProfileModel.Markup.OverviewBanner = GetMarkupResult(overviewBannerSegmnentMarkupTask, overviewBannerSegmentService.SegmentClientOptions);
            }

            if (refreshRelatedCareersSegment)
            {
                JobProfileModel.Data.RelatedCareers = GetDataResult(relatedCareersSegmnentDataTask);
                JobProfileModel.Markup.RelatedCareers = GetMarkupResult(relatedCareersSegmnentMarkupTask, relatedCareersSegmentService.SegmentClientOptions);
            }

            if (refreshWhatItTakesSegment)
            {
                JobProfileModel.Data.WhatItTakes = GetDataResult(whatItTakesSegmnentDataTask);
                JobProfileModel.Markup.WhatItTakes = GetMarkupResult(whatItTakesSegmnentMarkupTask, whatItTakesSegmentService.SegmentClientOptions);
            }

            if (refreshWhatYouWillDoSegment)
            {
                JobProfileModel.Data.WhatYouWillDo = GetDataResult(whatYouWillDoSegmnentDataTask);
                JobProfileModel.Markup.WhatYouWillDo = GetMarkupResult(whatYouWillDoSegmnentMarkupTask, whatYouWillDoSegmentService.SegmentClientOptions);
            }

            logger.LogInformation($"{nameof(LoadAsync)}: Loaded segments for {RefreshJobProfileSegmentModel.CanonicalName}");
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            var tasks = new List<Task<HealthCheckItems>>
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
            where TModel : ISegmentDataModel
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    return task.Result;
                }
            }

            return default(TModel);
        }

        private string GetMarkupResult(Task<string> task, SegmentClientOptions segmentClientOptions)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    var markup = UrlRewriter.Rewrite(task.Result, segmentClientOptions.BaseAddress, RequestBaseAddress);

                    return markup;
                }
            }

            return segmentClientOptions.OfflineHtml;
        }

        private IList<HealthCheckItem> GetHealthResults(Task<HealthCheckItems> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result?.HealthItems?.Count > 0)
                {
                    return task.Result.HealthItems;
                }
                else
                {
                    return new List<HealthCheckItem>()
                    {
                        new HealthCheckItem
                        {
                            Service = task.Result.Source.ToString(),
                            Message = $"No health response from {task.Result.Source.ToString()} app",
                        },
                    };
                }
            }

            return null;
        }
    }
}
