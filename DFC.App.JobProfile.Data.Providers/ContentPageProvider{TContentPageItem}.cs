using DFC.Compui.Cosmos.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    public abstract class ContentPageProvider<TContentPageItem> :
        IProvidePageContent<TContentPageItem>
            where TContentPageItem : class, IContentPageModel
    {
        protected ContentPageProvider(
            IContentPageService<TContentPageItem> pageService)
        {
            PageService = pageService;
        }

        internal IContentPageService<TContentPageItem> PageService { get; }

        public async Task<bool> Ping() =>
            await PageService.PingAsync().ConfigureAwait(false);

        // TODO: FIX ME! retrieve by document id doesn't work...
        public async Task<TContentPageItem> GetItemBy(Guid documentId) =>
            await GetItemBy(d => d.Id == documentId);

        public async Task<TContentPageItem> GetItemBy(string canonicalName) =>
            !string.IsNullOrWhiteSpace(canonicalName)
                ? await GetItemBy(d => d.CanonicalName == canonicalName)
                : throw new ArgumentNullException(nameof(canonicalName));

        internal async Task<TContentPageItem> GetItemBy(Expression<Func<TContentPageItem, bool>> expression) =>
            (await PageService.GetAsync(expression).ConfigureAwait(false))?.FirstOrDefault();
    }
}