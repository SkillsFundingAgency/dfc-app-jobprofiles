using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    public sealed class JobProfileProvider :
        ContentPageProvider<JobProfileCached>,
        IProvideJobProfiles
    {
        public JobProfileProvider(
            IContentPageService<JobProfileCached> pageService)
            : base(pageService)
        {
        }

        public async Task<IReadOnlyCollection<JobProfileCached>> GetAllItems() =>
            (await PageService.GetAllAsync().ConfigureAwait(false))?.ToList()
                ?? new List<JobProfileCached>();
    }
}