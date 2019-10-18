using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService.Utilities;
using Microsoft.AspNetCore.Html;
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

        public RefreshJobProfileSegment RefreshJobProfileSegmentModel { get; set; }

        public JobProfileModel JobProfileModel { get; set; }

        public Uri RequestBaseAddress { get; set; }

        public async Task<SegmentModel> RefreshSegmentAsync(RefreshJobProfileSegment toRefresh)
        {
            if (toRefresh is null)
            {
                throw new ArgumentNullException(nameof(toRefresh));
            }

            switch (toRefresh.Segment)
            {
                case JobProfileSegment.Overview:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.HowToBecome:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.WhatItTakes:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.WhatYouWillDo:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.CareerPathsAndProgression:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.CurrentOppurtunities:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.RelatedCareers:
                    return await GetSegmentDataAsync(overviewBannerSegmentService, toRefresh).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(toRefresh), $"Segment to be refreshed should be one of {Enum.GetNames(typeof(JobProfileSegment))}");
            }
        }

        /*
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
                careerPathSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                careerPathSegmnentDataTask = careerPathSegmentService.LoadDataAsync();
                tasks.Add(careerPathSegmnentDataTask);

                careerPathSegmnentMarkupTask = careerPathSegmentService.LoadMarkupAsync();
                tasks.Add(careerPathSegmnentMarkupTask);
            }

            if (refreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                currentOpportunitiesSegmnentDataTask = currentOpportunitiesSegmentService.LoadDataAsync();
                tasks.Add(currentOpportunitiesSegmnentDataTask);

                currentOpportunitiesSegmnentMarkupTask = currentOpportunitiesSegmentService.LoadMarkupAsync();
                tasks.Add(currentOpportunitiesSegmnentMarkupTask);
            }

            if (refreshHowToBecomeSegment)
            {
                howToBecomeSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                howToBecomeSegmnentDataTask = howToBecomeSegmentService.LoadDataAsync();
                tasks.Add(howToBecomeSegmnentDataTask);

                howToBecomeSegmnentMarkupTask = howToBecomeSegmentService.LoadMarkupAsync();
                tasks.Add(howToBecomeSegmnentMarkupTask);
            }

            if (refreshOverviewBannerSegment)
            {
                overviewBannerSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                overviewBannerSegmnentDataTask = overviewBannerSegmentService.LoadDataAsync();
                tasks.Add(overviewBannerSegmnentDataTask);

                overviewBannerSegmnentMarkupTask = overviewBannerSegmentService.LoadMarkupAsync();
                tasks.Add(overviewBannerSegmnentMarkupTask);
            }

            if (refreshRelatedCareersSegment)
            {
                relatedCareersSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                relatedCareersSegmnentDataTask = relatedCareersSegmentService.LoadDataAsync();
                tasks.Add(relatedCareersSegmnentDataTask);

                relatedCareersSegmnentMarkupTask = relatedCareersSegmentService.LoadMarkupAsync();
                tasks.Add(relatedCareersSegmnentMarkupTask);
            }

            if (refreshWhatItTakesSegment)
            {
                whatItTakesSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                whatItTakesSegmnentDataTask = whatItTakesSegmentService.LoadDataAsync();
                tasks.Add(whatItTakesSegmnentDataTask);

                whatItTakesSegmnentMarkupTask = whatItTakesSegmentService.LoadMarkupAsync();
                tasks.Add(whatItTakesSegmnentMarkupTask);
            }

            if (refreshWhatYouWillDoSegment)
            {
                whatYouWillDoSegmentService.DocumentId = RefreshJobProfileSegmentModel.JobProfileId;

                whatYouWillDoSegmnentDataTask = whatYouWillDoSegmentService.LoadDataAsync();
                tasks.Add(whatYouWillDoSegmnentDataTask);

                whatYouWillDoSegmnentMarkupTask = whatYouWillDoSegmentService.LoadMarkupAsync();
                tasks.Add(whatYouWillDoSegmnentMarkupTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            if (refreshCareerPathSegment)
            {
                JobProfileModel.Data.CareerPath = GetDataResult(careerPathSegmnentDataTask);
                JobProfileModel.Segments.CareerPath = GetMarkupResult(careerPathSegmnentMarkupTask, careerPathSegmentService.SegmentClientOptions);
            }

            if (refreshCurrentOpportunitiesSegment)
            {
                JobProfileModel.Data.CurrentOpportunities = GetDataResult(currentOpportunitiesSegmnentDataTask);
                JobProfileModel.Segments.CurrentOpportunities = GetMarkupResult(currentOpportunitiesSegmnentMarkupTask, currentOpportunitiesSegmentService.SegmentClientOptions);
            }

            if (refreshHowToBecomeSegment)
            {
                JobProfileModel.Data.HowToBecome = GetDataResult(howToBecomeSegmnentDataTask);
                JobProfileModel.Segments.HowToBecome = GetMarkupResult(howToBecomeSegmnentMarkupTask, howToBecomeSegmentService.SegmentClientOptions);
            }

            if (refreshOverviewBannerSegment)
            {
                JobProfileModel.Data.OverviewBanner = GetDataResult(overviewBannerSegmnentDataTask);
                JobProfileModel.Segments.OverviewBanner = GetMarkupResult(overviewBannerSegmnentMarkupTask, overviewBannerSegmentService.SegmentClientOptions);
            }

            if (refreshRelatedCareersSegment)
            {
                JobProfileModel.Data.RelatedCareers = GetDataResult(relatedCareersSegmnentDataTask);
                JobProfileModel.Segments.RelatedCareers = GetMarkupResult(relatedCareersSegmnentMarkupTask, relatedCareersSegmentService.SegmentClientOptions);
            }

            if (refreshWhatItTakesSegment)
            {
                JobProfileModel.Data.WhatItTakes = GetDataResult(whatItTakesSegmnentDataTask);
                JobProfileModel.Segments.WhatItTakes = GetMarkupResult(whatItTakesSegmnentMarkupTask, whatItTakesSegmentService.SegmentClientOptions);
            }

            if (refreshWhatYouWillDoSegment)
            {
                JobProfileModel.Data.WhatYouWillDo = GetDataResult(whatYouWillDoSegmnentDataTask);
                JobProfileModel.Segments.WhatYouWillDo = GetMarkupResult(whatYouWillDoSegmnentMarkupTask, whatYouWillDoSegmentService.SegmentClientOptions);
            }

            logger.LogInformation($"{nameof(LoadAsync)}: Loaded segments for {RefreshJobProfileSegmentModel.CanonicalName}");
        }
        */

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

        private async Task<SegmentModel> GetSegmentDataAsync<T>(IBaseSegmentService<T> segmentService, RefreshJobProfileSegment toRefresh)
                    where T : ISegmentDataModel
        {
            var jsonResultTask = segmentService.GetJsonAsync(toRefresh.JobProfileId);
            var htmlResultTask = segmentService.GetMarkupAsync(toRefresh.JobProfileId);

            await Task.WhenAll(jsonResultTask, htmlResultTask).ConfigureAwait(false);

            return new SegmentModel
            {
                Segment = toRefresh.Segment,
                RefreshedAt = DateTime.UtcNow,
                RefreshSequence = toRefresh.SequenceNumber,
                Markup = GetMarkupResult(htmlResultTask, segmentService.SegmentClientOptions),
                Json = jsonResultTask.Result,
            };
        }

        private HtmlString GetMarkupResult(Task<string> task, SegmentClientOptions segmentClientOptions)
        {
            if (task != null)
            {
                if (task.Result is null)
                {
                    return null;
                }

                if (task.IsCompletedSuccessfully)
                {
                    var markup = UrlRewriter.Rewrite(task.Result, segmentClientOptions.BaseAddress, RequestBaseAddress);
                    return new HtmlString(markup);
                }
            }

            return new HtmlString(segmentClientOptions.OfflineHtml);
        }
    }
}