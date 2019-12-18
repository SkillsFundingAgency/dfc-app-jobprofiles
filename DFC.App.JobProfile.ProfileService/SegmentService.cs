using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Contracts.SegmentServices;
using DFC.App.JobProfile.Data.HttpClientPolicies;
using DFC.App.JobProfile.Data.Models;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class SegmentService : Data.Contracts.ISegmentService
    {
        private readonly ILogService logService;
        private readonly ISegmentRefreshService<OverviewBannerSegmentClientOptions> overviewBannerSegmentService;
        private readonly ISegmentRefreshService<HowToBecomeSegmentClientOptions> howToBecomeSegmentService;
        private readonly ISegmentRefreshService<WhatItTakesSegmentClientOptions> whatItTakesSegmentService;
        private readonly ISegmentRefreshService<WhatYouWillDoSegmentClientOptions> whatYouWillDoSegmentService;
        private readonly ISegmentRefreshService<CareerPathSegmentClientOptions> careerPathSegmentService;
        private readonly ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions> currentOpportunitiesSegmentService;
        private readonly ISegmentRefreshService<RelatedCareersSegmentClientOptions> relatedCareersSegmentService;

        public SegmentService(
            ILogService logService,
            ISegmentRefreshService<OverviewBannerSegmentClientOptions> overviewBannerSegmentService,
            ISegmentRefreshService<HowToBecomeSegmentClientOptions> howToBecomeSegmentService,
            ISegmentRefreshService<WhatItTakesSegmentClientOptions> whatItTakesSegmentService,
            ISegmentRefreshService<WhatYouWillDoSegmentClientOptions> whatYouWillDoSegmentService,
            ISegmentRefreshService<CareerPathSegmentClientOptions> careerPathSegmentService,
            ISegmentRefreshService<CurrentOpportunitiesSegmentClientOptions> currentOpportunitiesSegmentService,
            ISegmentRefreshService<RelatedCareersSegmentClientOptions> relatedCareersSegmentService)
        {
            this.logService = logService;
            this.overviewBannerSegmentService = overviewBannerSegmentService;
            this.howToBecomeSegmentService = howToBecomeSegmentService;
            this.whatItTakesSegmentService = whatItTakesSegmentService;
            this.whatYouWillDoSegmentService = whatYouWillDoSegmentService;
            this.careerPathSegmentService = careerPathSegmentService;
            this.currentOpportunitiesSegmentService = currentOpportunitiesSegmentService;
            this.relatedCareersSegmentService = relatedCareersSegmentService;
        }

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
                    return await GetSegmentDataAsync(howToBecomeSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.WhatItTakes:
                    return await GetSegmentDataAsync(whatItTakesSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.WhatYouWillDo:
                    return await GetSegmentDataAsync(whatYouWillDoSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.CareerPathsAndProgression:
                    return await GetSegmentDataAsync(careerPathSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.CurrentOpportunities:
                    return await GetSegmentDataAsync(currentOpportunitiesSegmentService, toRefresh).ConfigureAwait(false);

                case JobProfileSegment.RelatedCareers:
                    return await GetSegmentDataAsync(relatedCareersSegmentService, toRefresh).ConfigureAwait(false);

                default:
                    throw new ArgumentOutOfRangeException(nameof(toRefresh), $"Segment to be refreshed should be one of {Enum.GetNames(typeof(JobProfileSegment))}");
            }
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

        public OfflineSegmentModel GetOfflineSegment(JobProfileSegment segment)
        {
            switch (segment)
            {
                case JobProfileSegment.Overview:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(overviewBannerSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.HowToBecome:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(howToBecomeSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.WhatItTakes:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(whatItTakesSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.WhatYouWillDo:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(whatYouWillDoSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.CareerPathsAndProgression:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(careerPathSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.CurrentOpportunities:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(currentOpportunitiesSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                case JobProfileSegment.RelatedCareers:
                    return new OfflineSegmentModel
                    {
                        OfflineMarkup = new HtmlString(relatedCareersSegmentService.SegmentClientOptions.OfflineHtml),
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(segment), $"Segment should be one of {Enum.GetNames(typeof(JobProfileSegment))}");
            }
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

        private async Task<SegmentModel> GetSegmentDataAsync<T>(ISegmentRefreshService<T> segmentService, RefreshJobProfileSegment toRefresh)
                    where T : SegmentClientOptions
        {
            var jsonResultTask = segmentService.GetJsonAsync(toRefresh.JobProfileId);
            var htmlResultTask = segmentService.GetMarkupAsync(toRefresh.JobProfileId);

            await Task.WhenAll(jsonResultTask, htmlResultTask).ConfigureAwait(false);

            return new SegmentModel
            {
                Segment = toRefresh.Segment,
                RefreshedAt = DateTime.UtcNow,
                RefreshSequence = toRefresh.SequenceNumber,
                Markup = htmlResultTask?.IsCompletedSuccessfully == true && htmlResultTask.Result != null ? new HtmlString(htmlResultTask.Result) : HtmlString.Empty,
                Json = jsonResultTask?.IsCompletedSuccessfully == true ? jsonResultTask.Result : null,
                RefreshStatus = jsonResultTask?.IsCompletedSuccessfully == true
                    && htmlResultTask?.IsCompletedSuccessfully == true
                    ? Data.Enums.RefreshStatus.Success
                    : Data.Enums.RefreshStatus.Failed,
            };
        }
    }
}