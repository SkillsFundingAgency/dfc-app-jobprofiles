using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class JobProfileService :
        IJobProfileService
    {
        private readonly IContentPageService<JobProfileCached> _pageService;
        private readonly IMapper _mapper;

        public JobProfileService(
            IContentPageService<JobProfileCached> pageService,
            IMapper mapper)
        {
            _pageService = pageService;
            _mapper = mapper;
        }

        public async Task<bool> Ping() =>
            await _pageService.PingAsync().ConfigureAwait(false);

        public async Task<IReadOnlyCollection<JobProfileCached>> GetAllItems() =>
            (await _pageService.GetAllAsync().ConfigureAwait(false))?.ToList()
                ?? new List<JobProfileCached>();

        public async Task<JobProfileCached> GetItemBy(Guid documentId) =>
            (await _pageService.GetAsync(d => d.Id == documentId).ConfigureAwait(false)).FirstOrDefault();

        public async Task<JobProfileCached> GetItemBy(string canonicalName)
        {
            if (!string.IsNullOrWhiteSpace(canonicalName))
            {
                return (await _pageService.GetAsync(d => d.CanonicalName == canonicalName).ConfigureAwait(false)).FirstOrDefault();
            }

            throw new ArgumentNullException(nameof(canonicalName));
        }

        // TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
        //public async Task<JobProfileCached> GetByAlternativeNameAsync(string alternativeName)
        //{
        //    if (string.IsNullOrWhiteSpace(alternativeName))
        //    {
        //        throw new ArgumentNullException(nameof(alternativeName));
        //    }

        //    return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        //}

        //public async Task<HttpStatusCode> Create(JobProfileCached jobProfileModel)
        //{
        //    if (jobProfileModel == null)
        //    {
        //        throw new ArgumentNullException(nameof(jobProfileModel));
        //    }

        //    jobProfileModel.MetaTags = jobProfileModel.MetaTags is null ? new MetaTags() : jobProfileModel.MetaTags;
        //    jobProfileModel.Segments = jobProfileModel.Segments is null ? new List<SegmentModel>() : jobProfileModel.Segments;

        //    var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
        //    if (existingRecord != null)
        //    {
        //        return HttpStatusCode.AlreadyReported;
        //    }

        //    return await repository.UpsertAsync(jobProfileModel).ConfigureAwait(false);
        //}

        //public async Task<HttpStatusCode> Update(JobProfileCached jobProfileMetadata)
        //{
        //    if (jobProfileMetadata is null)
        //    {
        //        throw new ArgumentNullException(nameof(jobProfileMetadata));
        //    }

        //    var existingRecord = await GetByIdAsync(jobProfileMetadata.JobProfileId).ConfigureAwait(false);
        //    if (existingRecord is null)
        //    {
        //        return HttpStatusCode.NotFound;
        //    }

        //    if (existingRecord.SequenceNumber > jobProfileMetadata.SequenceNumber)
        //    {
        //        return HttpStatusCode.AlreadyReported;
        //    }

        //    var mappedRecord = mapper.Map(jobProfileMetadata, existingRecord);
        //    return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        //}

        //public async Task<HttpStatusCode> Update(Data.Models.JobProfileModel jobProfileModel)
        //{
        //    if (jobProfileModel == null)
        //    {
        //        throw new ArgumentNullException(nameof(jobProfileModel));
        //    }

        //    var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
        //    if (existingRecord is null)
        //    {
        //        return HttpStatusCode.NotFound;
        //    }

        //    if (existingRecord.SequenceNumber > jobProfileModel.SequenceNumber)
        //    {
        //        return HttpStatusCode.AlreadyReported;
        //    }

        //    var mappedRecord = mapper.Map(jobProfileModel, existingRecord);
        //    return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        //}

        //public async Task<bool> DeleteAsync(Guid documentId)
        //{
        //    var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

        //    return result == HttpStatusCode.NoContent;
        //}
    }
}