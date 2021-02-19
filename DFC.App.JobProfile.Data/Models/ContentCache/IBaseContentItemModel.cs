using DFC.Content.Pkg.Netcore.Data.Models;
using System.Collections.Generic;

namespace DFC.Content.Pkg.Netcore.Data.Contracts
{
    public interface IBaseContentItemModel<TModel> : IApiDataModel
        where TModel : class
    {
        ContentLinksModel? ContentLinks { get; set; }

        IList<TModel> ContentItems { get; set; }
    }
}
