using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
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
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;

        public JobProfileService(
            ICosmosRepository<Data.Models.JobProfileModel> repository,
            IDraftJobProfileService draftJobProfileService,
            ISegmentService segmentService,
            IMapper mapper)
        {
            this.repository = repository;
            this.draftJobProfileService = draftJobProfileService;
            this.segmentService = segmentService;
            this.mapper = mapper;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            return await segmentService.SegmentsHealthCheckAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Data.Models.JobProfileModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return isDraft
                ? await draftJobProfileService.GetSitefinityData(canonicalName.ToLowerInvariant()).ConfigureAwait(false)
                : await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<Data.Models.JobProfileModel> GetByAlternativeNameAsync(string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                throw new ArgumentNullException(nameof(alternativeName));
            }

            return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Create(Data.Models.JobProfileModel jobProfileModel)
        {
            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            jobProfileModel.MetaTags ??= new MetaTags();
            jobProfileModel.Segments ??= new List<SegmentModel>();

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            return await repository.UpsertAsync(jobProfileModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(Data.Models.JobProfileModel jobProfileModel)
        {
            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            jobProfileModel.MetaTags ??= new MetaTags();
            jobProfileModel.Segments ??= new List<SegmentModel>();

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                return HttpStatusCode.NotFound;
            }

            var mappedRecord = mapper.Map(jobProfileModel, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment segmentRefresh)
        {
            if (segmentRefresh is null)
            {
                throw new ArgumentNullException(nameof(segmentRefresh));
            }

            //Check existing document
            var existingJobProfile = await GetByIdAsync(segmentRefresh.JobProfileId).ConfigureAwait(false);
            if (existingJobProfile is null)
            {
                return HttpStatusCode.NotFound;
            }

            var segmentData = await segmentService.RefreshSegmentAsync(segmentRefresh).ConfigureAwait(false);
            if (existingJobProfile.Segments.Any(s => s.Segment == segmentData.Segment))
            {
                var existingItem = existingJobProfile.Segments.Single(s => s.Segment == segmentData.Segment);
                var index = existingJobProfile.Segments.IndexOf(existingItem);
                segmentData.Markup ??= existingItem.Markup;
                segmentData.Json ??= existingItem.Json;
                existingJobProfile.Segments[index] = segmentData;
            }
            else
            {
                existingJobProfile.Segments.Add(segmentData);
            }

            return await repository.UpsertAsync(existingJobProfile).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}