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
                    var markup = UrlRewriter.Rewrite(task.Result, segmentClientOptions.BaseAddress, new Uri("/"));
                    return new HtmlString(markup);
                }
            }

            return new HtmlString(segmentClientOptions.OfflineHtml);
        }
    }
}