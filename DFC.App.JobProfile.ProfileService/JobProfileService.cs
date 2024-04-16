﻿using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.Logger.AppInsights.Contracts;
using System;
using System.Collections.Generic;
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

        public JobProfileService(
            ICosmosRepository<JobProfileModel> repository,
            ISegmentService segmentService,
            IMapper mapper,
            ILogService logService,
            ISharedContentRedisInterface sharedContentRedisInterface)
        {
            this.repository = repository;
            this.segmentService = segmentService;
            this.mapper = mapper;
            this.logService = logService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
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

            try
            {
                //Get the various job profile segments here.  Probably will need to use the URL as opposed to canonicalName.  We may need to update this call
                //higher up to pass in the correct strategy etc. For the moment I have left the database call in here so that we can compare data coming from the
                //databasae vs the NuGet package.  At the moment, I'm using a GetData call, however we must use the GetDataAsyncWithExpiry method for all
                //JobProfile calls going forward.
                var response = await sharedContentRedisInterface.GetDataAsync<JobProfilesOverviewResponse>(canonicalName, ApplicationKeys.JobProfilesOverview);

                //etc...
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
            }

            return await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
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