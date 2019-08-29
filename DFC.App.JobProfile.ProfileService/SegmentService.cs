using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class SegmentService : ISegmentService
    {
        private readonly ISegmentLoadService<CareerPathSegmentModel> careerPathSegmentLoadService;
        private readonly ISegmentLoadService<CurrentOpportunitiesSegmentModel> currentOpportunitiesSegmentLoadService;
        private readonly ISegmentLoadService<HowToBecomeSegmentModel> howToBecomeSegmentLoadService;
        private readonly ISegmentLoadService<OverviewBannerSegmentModel> overviewBannerSegmentLoadService;
        private readonly ISegmentLoadService<RelatedCareersSegmentModel> relatedCareersSegmentLoadService;
        private readonly ISegmentLoadService<WhatItTakesSegmentModel> whatItTakesSegmentLoadService;

        public SegmentService(
            ISegmentLoadService<CareerPathSegmentModel> careerPathSegmentLoadService,
            ISegmentLoadService<CurrentOpportunitiesSegmentModel> currentOpportunitiesSegmentLoadService,
            ISegmentLoadService<HowToBecomeSegmentModel> howToBecomeSegmentLoadService,
            ISegmentLoadService<OverviewBannerSegmentModel> overviewBannerSegmentLoadService,
            ISegmentLoadService<RelatedCareersSegmentModel> relatedCareersSegmentLoadService,
            ISegmentLoadService<WhatItTakesSegmentModel> whatItTakesSegmentLoadService)
        {
            this.careerPathSegmentLoadService = careerPathSegmentLoadService;
            this.currentOpportunitiesSegmentLoadService = currentOpportunitiesSegmentLoadService;
            this.howToBecomeSegmentLoadService = howToBecomeSegmentLoadService;
            this.overviewBannerSegmentLoadService = overviewBannerSegmentLoadService;
            this.relatedCareersSegmentLoadService = relatedCareersSegmentLoadService;
            this.whatItTakesSegmentLoadService = whatItTakesSegmentLoadService;
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
                careerPathSegmnentTask = careerPathSegmentLoadService.LoadAsync();
                tasks.Add(careerPathSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshCurrentOpportunitiesSegment)
            {
                currentOpportunitiesSegmnentTask = currentOpportunitiesSegmentLoadService.LoadAsync();
                tasks.Add(currentOpportunitiesSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshHowToBecomeSegment)
            {
                howToBecomeSegmnentTask = howToBecomeSegmentLoadService.LoadAsync();
                tasks.Add(howToBecomeSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshOverviewBannerSegment)
            {
                overviewBannerSegmnentTask = overviewBannerSegmentLoadService.LoadAsync();
                tasks.Add(overviewBannerSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshRelatedCareersSegment)
            {
                relatedCareersSegmnentTask = relatedCareersSegmentLoadService.LoadAsync();
                tasks.Add(relatedCareersSegmnentTask);
            }

            if (CreateOrUpdateJobProfileModel.RefreshAllSegments || CreateOrUpdateJobProfileModel.RefreshWhatItTakesSegment)
            {
                whatItTakesSegmnentTask = whatItTakesSegmentLoadService.LoadAsync();
                tasks.Add(whatItTakesSegmnentTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            JobProfileModel.Segments.CareerPath = GetResult(careerPathSegmnentTask);
            JobProfileModel.Segments.CurrentOpportunities = GetResult(currentOpportunitiesSegmnentTask);
            JobProfileModel.Segments.HowToBecome = GetResult(howToBecomeSegmnentTask);
            JobProfileModel.Segments.OverviewBanner = GetResult(overviewBannerSegmnentTask);
            JobProfileModel.Segments.RelatedCareers = GetResult(relatedCareersSegmnentTask);
            JobProfileModel.Segments.WhatItTakes = GetResult(whatItTakesSegmnentTask);
        }

        private CareerPathSegmentModel GetResult(Task<CareerPathSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }

        private CurrentOpportunitiesSegmentModel GetResult(Task<CurrentOpportunitiesSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }

        private HowToBecomeSegmentModel GetResult(Task<HowToBecomeSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }

        private OverviewBannerSegmentModel GetResult(Task<OverviewBannerSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }

        private RelatedCareersSegmentModel GetResult(Task<RelatedCareersSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }

        private WhatItTakesSegmentModel GetResult(Task<WhatItTakesSegmentModel> task)
        {
            if (task != null)
            {
                if (task.IsCompletedSuccessfully)
                {
                    return task.Result;
                }
            }

            return null;
        }
    }
}
