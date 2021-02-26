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
        IProvideJobProfiles
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
    }
}