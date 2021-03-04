using DFC.App.Services.Common.Registration;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    internal abstract class ContentPageProvider<TContentPageItem> :
        IProvidePageContent<TContentPageItem>,
        IRequireServiceRegistration
            where TContentPageItem : class, IContentPageModel
    {
        protected ContentPageProvider(
            IContentPageService<TContentPageItem> pageService)
        {
            PageService = pageService;
        }

        internal IContentPageService<TContentPageItem> PageService { get; }

        public Task<bool> Ping() =>
            PageService.PingAsync();

        public Task<TContentPageItem> GetItemBy(Guid documentId) =>
            PageService.GetByIdAsync(documentId);

        public async Task<TContentPageItem> GetItemBy(string canonicalName) =>
            !string.IsNullOrWhiteSpace(canonicalName)
                ? await GetItemBy(d => d.CanonicalName == canonicalName)
                : throw new ArgumentNullException(nameof(canonicalName));

        internal async Task<TContentPageItem> GetItemBy(Expression<Func<TContentPageItem, bool>> expression) =>
            (await PageService.GetAsync(expression))?.FirstOrDefault();
    }
}