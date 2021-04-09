using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    [ExcludeFromCodeCoverage]
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
            (await PageService.GetAllAsync())?.ToList()
                ?? new List<StaticItemCached>();
    }
}