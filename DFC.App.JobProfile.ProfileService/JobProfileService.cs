using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class JobProfileService : IJobProfileService
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;

        public JobProfileService(
            ICosmosRepository<JobProfileModel> repository,
            ISegmentService segmentService,
            IMapper mapper,
            ILogService logService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IRazorTemplateEngine razorTemplateEngine)
        {
            this.repository = repository;
            this.segmentService = segmentService;
            this.mapper = mapper;
            this.logService = logService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            return await segmentService.SegmentsHealthCheckAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobProfileModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<JobProfileModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<JobProfileModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            var howToBecome = new SegmentModel();

            try
            {
                howToBecome = await GetHowToBecomeSegmentAsync(canonicalName);

                var data = await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);

                if (data != null)
                {
                    data.Segments[2] = howToBecome;
                }

                return data;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }
        }

        private async Task<SegmentModel> GetHowToBecomeSegmentAsync(string canonicalName)
        {
            var howToBecome = new SegmentModel();

            try
            {
                // Get the response from GraphQl
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(ApplicationKeys.JobProfileHowToBecome + "/" + canonicalName, "PUBLISHED");

                // Map the response to a HowToBecomeSegmentDataModel
                var mappedResponse = mapper.Map<HowToBecomeSegmentDataModel>(response);

                // Map CommonRoutes for College
                var collegeCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.College);

                // Map CommonRoutes for University
                var universityCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.University);

                // Map CommonRoutes for Apprenticeship
                var apprenticeshipCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.Apprenticeship);

                // Combine CommonRoutes into a list
                var allCommonRoutes = new List<CommonRoutes>
                {
                    collegeCommonRoutes,
                    universityCommonRoutes,
                    apprenticeshipCommonRoutes,
                };

                // Combine the CommonRoutes with the mapped response
                mappedResponse.EntryRoutes.CommonRoutes = allCommonRoutes;

                // Serialize the mapped response into an object
                var howToBecomeObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },
                });

                // Render the CSHTML to string
                var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/HowToBecome/Body.cshtml", mappedResponse).ConfigureAwait(false);

                // Build the SegmentModel
                howToBecome = new SegmentModel
                {
                    Segment = Data.JobProfileSegment.HowToBecome,
                    Markup = new HtmlString(html),
                    JsonV1 = howToBecomeObject,
                    RefreshStatus = RefreshStatus.Success,
                };
            }
            catch (Exception e)
            {
                logService.LogError(e.ToString());
                throw;
            }

            return howToBecome;
        }

        public async Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                throw new ArgumentNullException(nameof(alternativeName));
            }

            return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Create(JobProfileModel jobProfileModel)
        {

            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            jobProfileModel.MetaTags = jobProfileModel.MetaTags is null ? new MetaTags() : jobProfileModel.MetaTags;
            jobProfileModel.Segments = jobProfileModel.Segments is null ? new List<SegmentModel>() : jobProfileModel.Segments;

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            return await repository.UpsertAsync(jobProfileModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(JobProfileMetadata jobProfileMetadata)
        {
            if (jobProfileMetadata is null)
            {
                throw new ArgumentNullException(nameof(jobProfileMetadata));
            }

            var existingRecord = await GetByIdAsync(jobProfileMetadata.JobProfileId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingRecord.SequenceNumber > jobProfileMetadata.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var mappedRecord = mapper.Map(jobProfileMetadata, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(JobProfileModel jobProfileModel)
        {
            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingRecord.SequenceNumber > jobProfileModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var mappedRecord = mapper.Map(jobProfileModel, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel)
        {
            if (refreshJobProfileSegmentModel is null)
            {
                throw new ArgumentNullException(nameof(refreshJobProfileSegmentModel));
            }

            //Check existing document
            var existingJobProfile = await GetByIdAsync(refreshJobProfileSegmentModel.JobProfileId).ConfigureAwait(false);
            if (existingJobProfile is null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingItem = existingJobProfile.Segments.SingleOrDefault(s => s.Segment == refreshJobProfileSegmentModel.Segment);
            if (existingItem?.RefreshSequence > refreshJobProfileSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var offlineSegmentData = segmentService.GetOfflineSegment(refreshJobProfileSegmentModel.Segment);
            var segmentData = await segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);
            if (existingItem is null)
            {
                segmentData.Markup = !string.IsNullOrEmpty(segmentData.Markup?.Value) ? segmentData.Markup : offlineSegmentData.OfflineMarkup;
                segmentData.Json ??= offlineSegmentData.OfflineJson;
                existingJobProfile.Segments.Add(segmentData);
            }
            else
            {
                var index = existingJobProfile.Segments.IndexOf(existingItem);
                var fallbackMarkup = !string.IsNullOrEmpty(existingItem.Markup?.Value) ? existingItem.Markup : offlineSegmentData.OfflineMarkup;
                segmentData.Markup = !string.IsNullOrEmpty(segmentData.Markup?.Value) ? segmentData.Markup : fallbackMarkup;
                segmentData.Json ??= existingItem.Json ?? offlineSegmentData.OfflineJson;

                existingJobProfile.Segments[index] = segmentData;
            }

            var result = await repository.UpsertAsync(existingJobProfile).ConfigureAwait(false);
            return segmentData.RefreshStatus == Data.Enums.RefreshStatus.Success ? result : HttpStatusCode.FailedDependency;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}