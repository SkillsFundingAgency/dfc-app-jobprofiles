using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    internal sealed class JobProfileProvider :
        ContentPageProvider<JobProfileCached>,
        IProvideJobProfiles
    {
        public JobProfileProvider(
            IContentPageService<JobProfileCached> pageService)
            : base(pageService)
        {
        }

        public async Task<IReadOnlyCollection<JobProfileCached>> GetAllItems() =>
            (await PageService.GetAllAsync())?.ToList()
                ?? new List<JobProfileCached>();
    }
}