using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    internal sealed class SharedContentProvider :
        ContentPageProvider<StaticItemCached>,
        IProvideSharedContent
    {
        public SharedContentProvider(
            IContentPageService<StaticItemCached> pageService)
            : base(pageService)
        {
        }

        public async Task<IReadOnlyCollection<StaticItemCached>> GetAllItems() =>
            (await PageService.GetAllAsync().ConfigureAwait(false))?.ToList()
                ?? new List<StaticItemCached>();
    }
}