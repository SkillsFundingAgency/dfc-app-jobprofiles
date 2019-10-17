using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segments;
using DFC.App.JobProfile.Data.Models.ServiceBusModels;
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

        public JobProfileService(
            ICosmosRepository<Data.Models.JobProfileModel> repository,
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

        public async Task<HttpStatusCode> UpsertAsync(Data.Models.JobProfileModel jobProfileModel)
        {
            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            if (jobProfileModel.MetaTags == null)
            {
                jobProfileModel.MetaTags = new MetaTags();
            }

            if (jobProfileModel.Markup == null)
            {
                jobProfileModel.Markup = new SegmentsMarkupModel();
            }

            if (jobProfileModel.Data == null)
            {
                jobProfileModel.Data = new SegmentsDataModel();
            }

            var result = await repository.UpsertAsync(jobProfileModel).ConfigureAwait(false);

            return result;
        }

        public async Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel, Data.Models.JobProfileModel existingJobProfileModel, Uri requestBaseAddress)
        {
            if (refreshJobProfileSegmentModel == null)
            {
                throw new ArgumentNullException(nameof(refreshJobProfileSegmentModel));
            }

            if (existingJobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(existingJobProfileModel));
            }

            if (requestBaseAddress == null)
            {
                throw new ArgumentNullException(nameof(requestBaseAddress));
            }

            if (existingJobProfileModel.MetaTags == null)
            {
                existingJobProfileModel.MetaTags = new MetaTags();
            }

            if (existingJobProfileModel.Markup == null)
            {
                existingJobProfileModel.Markup = new SegmentsMarkupModel();
            }

            if (existingJobProfileModel.Data == null)
            {
                existingJobProfileModel.Data = new SegmentsDataModel();
            }

            segmentService.RefreshJobProfileSegmentModel = refreshJobProfileSegmentModel;
            segmentService.JobProfileModel = existingJobProfileModel;
            segmentService.RequestBaseAddress = requestBaseAddress;

            await segmentService.LoadAsync().ConfigureAwait(false);

            var result = await repository.UpsertAsync(existingJobProfileModel).ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }
    }
}
