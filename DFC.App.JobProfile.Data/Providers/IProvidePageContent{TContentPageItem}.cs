using DFC.Compui.Cosmos.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    public interface IProvidePageContent<TContentPageItem>
        where TContentPageItem : class, IContentPageModel
    {
        Task<TContentPageItem> GetItemBy(Guid documentId);

        Task<TContentPageItem> GetItemBy(string canonicalName);

        Task<bool> Ping();
    }
}