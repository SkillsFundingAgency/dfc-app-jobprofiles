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
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly ISegmentService segmentService;

        public JobProfileService(
            ICosmosRepository<JobProfileModel> repository,
            IDraftJobProfileService draftJobProfileService,
            ISegmentService segmentService)
        {
            this.repository = repository;
            this.draftJobProfileService = draftJobProfileService;
            this.segmentService = segmentService;
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

        public async Task<JobProfileModel> GetByNameAsync(string canonicalName, bool isDraft = false)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return isDraft
                ? await draftJobProfileService.GetSitefinityData(canonicalName.ToLowerInvariant()).ConfigureAwait(false)
                : await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                throw new ArgumentNullException(nameof(alternativeName));
            }

            return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        }

        public async Task<JobProfileModel> CreateAsync(CreateOrUpdateJobProfileModel createJobProfileModel, Uri requestBaseAddress)
        {
            if (createJobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(createJobProfileModel));
            }

            if (requestBaseAddress == null)
            {
                throw new ArgumentNullException(nameof(requestBaseAddress));
            }

            var jobProfileModel = new JobProfileModel
            {
                DocumentId = createJobProfileModel.DocumentId,
                CanonicalName = createJobProfileModel.CanonicalName,
                Markup = new SegmentsMarkupModel(),
                Data = new SegmentsDataModel(),
            };

            segmentService.CreateOrUpdateJobProfileModel = createJobProfileModel;
            segmentService.JobProfileModel = jobProfileModel;
            segmentService.RequestBaseAddress = requestBaseAddress;

            await segmentService.LoadAsync().ConfigureAwait(false);

            var result = await repository.CreateAsync(jobProfileModel).ConfigureAwait(false);

            return result == HttpStatusCode.Created
                ? await GetByIdAsync(createJobProfileModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<JobProfileModel> ReplaceAsync(CreateOrUpdateJobProfileModel replaceJobProfileModel, JobProfileModel existingJobProfileModel, Uri requestBaseAddress)
        {
            if (replaceJobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(replaceJobProfileModel));
            }

            if (existingJobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(existingJobProfileModel));
            }

            if (requestBaseAddress == null)
            {
                throw new ArgumentNullException(nameof(requestBaseAddress));
            }

            if (existingJobProfileModel.Markup == null)
            {
                existingJobProfileModel.Markup = new SegmentsMarkupModel();
            }

            if (existingJobProfileModel.Data == null)
            {
                existingJobProfileModel.Data = new SegmentsDataModel();
            }

            segmentService.CreateOrUpdateJobProfileModel = replaceJobProfileModel;
            segmentService.JobProfileModel = existingJobProfileModel;
            segmentService.RequestBaseAddress = requestBaseAddress;

            await segmentService.LoadAsync().ConfigureAwait(false);

            var result = await repository.UpdateAsync(existingJobProfileModel.DocumentId, existingJobProfileModel).ConfigureAwait(false);

            return result == HttpStatusCode.OK
                ? await GetByIdAsync(replaceJobProfileModel.DocumentId).ConfigureAwait(false)
                : null;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}
