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

        Task<TRoot> GetComposedItem<TRoot, TBranch>(Uri uri)
            where TRoot : class, IRootContentItem<TBranch>, new()
            where TBranch : class, IBranchContentItem<TBranch>, new();

        Task<TBranch> GetBranchItem<TBranch>(Uri uri)
            where TBranch : class, IBranchContentItem<TBranch>, new();

        Task<IReadOnlyCollection<TApiModel>> GetStaticItems<TApiModel>()
            where TApiModel : class;
    }
}
