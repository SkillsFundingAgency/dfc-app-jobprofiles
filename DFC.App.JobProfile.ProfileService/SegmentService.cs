using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class SegmentService : ISegmentService
    {
        private readonly ICareerPathSegmentService careerPathSegmentService;
        private readonly ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService;
        private readonly IHowToBecomeSegmentService howToBecomeSegmentService;
        private readonly IOverviewBannerSegmentService overviewBannerSegmentService;
        private readonly IRelatedCareersSegmentService relatedCareersSegmentService;
        private readonly IWhatItTakesSegmentService whatItTakesSegmentService;

        public SegmentService(
            ICareerPathSegmentService careerPathSegmentService,
            ICurrentOpportunitiesSegmentService currentOpportunitiesSegmentService,
            IHowToBecomeSegmentService howToBecomeSegmentService,
            IOverviewBannerSegmentService overviewBannerSegmentService,
            IRelatedCareersSegmentService relatedCareersSegmentService,
            IWhatItTakesSegmentService whatItTakesSegmentService)
        {
            this.careerPathSegmentService = careerPathSegmentService;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.howToBecomeSegmentService = howToBecomeSegmentService;
            this.overviewBannerSegmentService = overviewBannerSegmentService;
            this.relatedCareersSegmentService = relatedCareersSegmentService;
            this.whatItTakesSegmentService = whatItTakesSegmentService;
        }

        public CreateOrUpdateJobProfileModel CreateOrUpdateJobProfileModel { get; set; }

        public JobProfileModel JobProfileModel { get; set; }

        public async Task LoadAsync()
        {
            var tasks = new List<Task>();

            Task<CareerPathSegmentModel> careerPathSegmnentTask = null;
            Task<CurrentOpportunitiesSegmentModel> currentOpportunitiesSegmnentTask = null;
            Task<HowToBecomeSegmentModel> howToBecomeSegmnentTask = null;
            Task<OverviewBannerSegmentModel> overviewBannerSegmnentTask = null;
            Task<RelatedCareersSegmentModel> relatedCareersSegmnentTask = null;
            Task<WhatItTakesSegmentModel> whatItTakesSegmnentTask = null;

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshCareerPathSegment)
            {
                careerPathSegmnentTask = careerPathSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(careerPathSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmnentTask = currentOpportunitiesSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(currentOpportunitiesSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshHowToBecomeSegment)
            {
                howToBecomeSegmnentTask = howToBecomeSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(howToBecomeSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshOverviewBannerSegment)
            {
                overviewBannerSegmnentTask = overviewBannerSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(overviewBannerSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshRelatedCareersSegment)
            {
                relatedCareersSegmnentTask = relatedCareersSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(relatedCareersSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshWhatItTakesSegment)
            {
                whatItTakesSegmnentTask = whatItTakesSegmentService.LoadAsync(CreateOrUpdateJobProfileModel.CanonicalName);
                tasks.Add(whatItTakesSegmnentTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            JobProfileModel.Segments.CareerPath = GetResult(careerPathSegmnentTask, careerPathSegmentService.SegmentClientOptions);
            JobProfileModel.Segments.CurrentOpportunities = GetResult(currentOpportunitiesSegmnentTask, currentOpportunitiesSegmentService.SegmentClientOptions);
            JobProfileModel.Segments.HowToBecome = GetResult(howToBecomeSegmnentTask, howToBecomeSegmentService.SegmentClientOptions);
            JobProfileModel.Segments.OverviewBanner = GetResult(overviewBannerSegmnentTask, overviewBannerSegmentService.SegmentClientOptions);
            JobProfileModel.Segments.RelatedCareers = GetResult(relatedCareersSegmnentTask, relatedCareersSegmentService.SegmentClientOptions);
            JobProfileModel.Segments.WhatItTakes = GetResult(whatItTakesSegmnentTask, whatItTakesSegmentService.SegmentClientOptions);
        }

        private TModel GetResult<TModel>(Task<TModel> task, SegmentClientOptions segmentClientOptions)
            where TModel : BaseSegmentModel, new()
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully && task.Result != null)
                {
                    return task.Result;
                }
            }

            var noResultsmodel = new TModel
            {
                Content = segmentClientOptions.OfflineHtml,
                LastReviewed = DateTime.UtcNow,
            };

            return noResultsmodel;
        }
    }
}
