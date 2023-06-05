using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
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
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly ILogService logService;

        public JobProfileService(
            ICosmosRepository<Data.Models.JobProfileModel> repository,
            ISegmentService segmentService,
            IMapper mapper)
        {
            this.repository = repository;
            this.segmentService = segmentService;
            this.mapper = mapper;
        }

        public async Task<bool> PingAsync()
        {
            logService.LogInformation($"{nameof(PingAsync)} has been called");

            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            logService.LogInformation($"{nameof(SegmentsHealthCheckAsync)} has been called");

            return await segmentService.SegmentsHealthCheckAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Data.Models.JobProfileModel>> GetAllAsync()
        {
            logService.LogInformation($"{nameof(GetAllAsync)} has been called");

            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByIdAsync(Guid documentId)
        {
            logService.LogInformation($"{nameof(GetByIdAsync)} has been called");

            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByNameAsync(string canonicalName)
        {
            logService.LogInformation($"{nameof(GetByNameAsync)} has been called");

            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByAlternativeNameAsync(string alternativeName)
        {
            logService.LogInformation($"{nameof(GetByAlternativeNameAsync)} has been called with alternativeName {nameof(alternativeName)}");

            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                throw new ArgumentNullException(nameof(alternativeName));
            }

            return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Create(Data.Models.JobProfileModel jobProfileModel)
        {
            logService.LogInformation($"{nameof(Create)} has been called with JobProfileModel {nameof(jobProfileModel)}");

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

        public async Task<HttpStatusCode> Update(Data.Models.JobProfileMetadata jobProfileMetadata)
        {
            logService.LogInformation($"{nameof(Update)} has been called with JobProfileMetadata {nameof(jobProfileMetadata)}");

            if (jobProfileMetadata is null)
            {
                throw new ArgumentNullException(nameof(jobProfileMetadata));
            }

            var existingRecord = await GetByIdAsync(jobProfileMetadata.JobProfileId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                logService.LogWarning($"{nameof(GetByIdAsync)} has been called with JobProfileId {nameof(jobProfileMetadata.JobProfileId)} " +
                    $"and returned null");

                return HttpStatusCode.NotFound;
            }

            if (existingRecord.SequenceNumber > jobProfileMetadata.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var mappedRecord = mapper.Map(jobProfileMetadata, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(Data.Models.JobProfileModel jobProfileModel)
        {
            logService.LogInformation($"{nameof(Update)} has been called with JobProfileModel {nameof(jobProfileModel)}");

            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                logService.LogWarning($"{nameof(GetByIdAsync)} has been called with DocumentId {nameof(jobProfileModel.JobProfileId)} " +
                    $"and returned null");

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
            logService.LogInformation($"{nameof(refreshJobProfileSegmentModel)} has been called with RefreshJobProfileSegment {nameof(refreshJobProfileSegmentModel)}");

            if (refreshJobProfileSegmentModel is null)
            {
                throw new ArgumentNullException(nameof(refreshJobProfileSegmentModel));
            }

            //Check existing document
            var existingJobProfile = await GetByIdAsync(refreshJobProfileSegmentModel.JobProfileId).ConfigureAwait(false);
            if (existingJobProfile is null)
            {
                logService.LogWarning($"{nameof(GetByIdAsync)} has been called with JobProfileId {nameof(refreshJobProfileSegmentModel.JobProfileId)} " +
                    $"and returned null");

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
                logService.LogWarning($"{nameof(existingJobProfile.Segments)} has been called and returned null");

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
            logService.LogInformation($"{nameof(DeleteAsync)} has been called with documentId {nameof(documentId)}");

            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}