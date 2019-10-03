using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.Data.Models.Segments.CareerPathModels;
using DFC.App.JobProfile.Data.Models.Segments.CurrentOpportunitiesModels;
using DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels;
using DFC.App.JobProfile.Data.Models.Segments.OverviewBannerModels;
using DFC.App.JobProfile.Data.Models.Segments.RelatedCareersModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatItTakesModels;
using DFC.App.JobProfile.Data.Models.Segments.WhatYouWillDoModels;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
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

        public RefreshJobProfileSegment RefreshJobProfileSegment { get; set; }

        public JobProfileModel JobProfileModel { get; set; }

        public Uri RequestBaseAddress { get; set; }

        public async Task LoadAsync()
        {
            logger.LogInformation($"{nameof(LoadAsync)}: Loading segments for {RefreshJobProfileSegment.CanonicalName}");

            var tasks = new List<Task>();

            bool refreshAllSegments = string.IsNullOrWhiteSpace(RefreshJobProfileSegment.Segment);
            bool refreshCareerPathSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == CareerPathSegmentModel.SegmentName;
            bool refreshCurrentOpportunitiesSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == CurrentOpportunitiesSegmentModel.SegmentName;
            bool refreshHowToBecomeSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == HowToBecomeSegmentModel.SegmentName;
            bool refreshOverviewBannerSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == OverviewBannerSegmentModel.SegmentName;
            bool refreshRelatedCareersSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == RelatedCareersSegmentModel.SegmentName;
            bool refreshWhatItTakesSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == WhatItTakesSegmentModel.SegmentName;
            bool refreshWhatYouWillDoSegment = refreshAllSegments || RefreshJobProfileSegment.Segment == WhatYouWillDoSegmentModel.SegmentName;

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

            if (refreshAllSegments || refreshCareerPathSegment)
            {
                careerPathSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                careerPathSegmnentDataTask = careerPathSegmentService.LoadDataAsync();
                tasks.Add(careerPathSegmnentDataTask);

                careerPathSegmnentMarkupTask = careerPathSegmentService.LoadMarkupAsync();
                tasks.Add(careerPathSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                currentOpportunitiesSegmnentDataTask = currentOpportunitiesSegmentService.LoadDataAsync();
                tasks.Add(currentOpportunitiesSegmnentDataTask);

                currentOpportunitiesSegmnentMarkupTask = currentOpportunitiesSegmentService.LoadMarkupAsync();
                tasks.Add(currentOpportunitiesSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshHowToBecomeSegment)
            {
                howToBecomeSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                howToBecomeSegmnentDataTask = howToBecomeSegmentService.LoadDataAsync();
                tasks.Add(howToBecomeSegmnentDataTask);

                howToBecomeSegmnentMarkupTask = howToBecomeSegmentService.LoadMarkupAsync();
                tasks.Add(howToBecomeSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshOverviewBannerSegment)
            {
                overviewBannerSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                overviewBannerSegmnentDataTask = overviewBannerSegmentService.LoadDataAsync();
                tasks.Add(overviewBannerSegmnentDataTask);

                overviewBannerSegmnentMarkupTask = overviewBannerSegmentService.LoadMarkupAsync();
                tasks.Add(overviewBannerSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshRelatedCareersSegment)
            {
                relatedCareersSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                relatedCareersSegmnentDataTask = relatedCareersSegmentService.LoadDataAsync();
                tasks.Add(relatedCareersSegmnentDataTask);

                relatedCareersSegmnentMarkupTask = relatedCareersSegmentService.LoadMarkupAsync();
                tasks.Add(relatedCareersSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshWhatItTakesSegment)
            {
                whatItTakesSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                whatItTakesSegmnentDataTask = whatItTakesSegmentService.LoadDataAsync();
                tasks.Add(whatItTakesSegmnentDataTask);

                whatItTakesSegmnentMarkupTask = whatItTakesSegmentService.LoadMarkupAsync();
                tasks.Add(whatItTakesSegmnentMarkupTask);
            }

            if (refreshAllSegments || refreshWhatYouWillDoSegment)
            {
                whatYouWillDoSegmentService.DocumentId = RefreshJobProfileSegment.JobProfileId;

                whatYouWillDoSegmnentDataTask = whatYouWillDoSegmentService.LoadDataAsync();
                tasks.Add(whatYouWillDoSegmnentDataTask);

                whatYouWillDoSegmnentMarkupTask = whatYouWillDoSegmentService.LoadMarkupAsync();
                tasks.Add(whatYouWillDoSegmnentMarkupTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            JobProfileModel.Data.CareerPath = GetDataResult(careerPathSegmnentDataTask)?.Data;
            JobProfileModel.Data.CurrentOpportunities = GetDataResult(currentOpportunitiesSegmnentDataTask)?.Data;
            JobProfileModel.Data.HowToBecome = GetDataResult(howToBecomeSegmnentDataTask)?.Data;
            JobProfileModel.Data.OverviewBanner = GetDataResult(overviewBannerSegmnentDataTask)?.Data;
            JobProfileModel.Data.RelatedCareers = GetDataResult(relatedCareersSegmnentDataTask)?.Data;
            JobProfileModel.Data.WhatItTakes = GetDataResult(whatItTakesSegmnentDataTask)?.Data;
            JobProfileModel.Data.WhatYouWillDo = GetDataResult(whatYouWillDoSegmnentDataTask)?.Data;

            JobProfileModel.Markup.CareerPath = GetMarkupResult(careerPathSegmnentMarkupTask, careerPathSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.CurrentOpportunities = GetMarkupResult(currentOpportunitiesSegmnentMarkupTask, currentOpportunitiesSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.HowToBecome = GetMarkupResult(howToBecomeSegmnentMarkupTask, howToBecomeSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.OverviewBanner = GetMarkupResult(overviewBannerSegmnentMarkupTask, overviewBannerSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.RelatedCareers = GetMarkupResult(relatedCareersSegmnentMarkupTask, relatedCareersSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.WhatItTakes = GetMarkupResult(whatItTakesSegmnentMarkupTask, whatItTakesSegmentService.SegmentClientOptions);
            JobProfileModel.Markup.WhatYouWillDo = GetMarkupResult(whatYouWillDoSegmnentMarkupTask, whatYouWillDoSegmentService.SegmentClientOptions);

            var title = JobProfileModel.Data.OverviewBanner?.Title ?? JobProfileModel.CanonicalName;

            JobProfileModel.MetaTags.Title = $"{title} | Explore careers";
            JobProfileModel.MetaTags.Description = JobProfileModel.Data.OverviewBanner?.Overview ?? title;
            JobProfileModel.MetaTags.Keywords = title;

            JobProfileModel.LastReviewed = DateTime.UtcNow;

            logger.LogInformation($"{nameof(LoadAsync)}: Loaded segments for {RefreshJobProfileSegment.CanonicalName}");
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
                LastReviewed = DateTime.UtcNow,
            };

            return noResultsModel;
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
