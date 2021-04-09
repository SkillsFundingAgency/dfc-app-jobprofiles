using DFC.App.JobProfile.ContentAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ContentAPI.Services
{
    public interface IProvideGraphContent
    {
        Task<IReadOnlyCollection<TApiModel>> GetSummaryItems<TApiModel>()
            where TApiModel : class, IGraphSummaryItem;

        Task<TRoot> GetContentItem<TRoot, TBranch>(Uri uri)
            where TRoot : class, IRootContentItem<TBranch>, new()
            where TBranch : class, ILinkedContentItem<TBranch>, new();

        Task<TBranch> GetLinkedItem<TBranch>(Uri uri)
            where TBranch : class, ILinkedContentItem<TBranch>, new();

        Task<IReadOnlyCollection<TApiModel>> GetStaticItems<TApiModel>()
            where TApiModel : class;
    }
}
