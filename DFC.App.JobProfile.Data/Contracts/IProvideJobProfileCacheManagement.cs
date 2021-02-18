using DFC.Content.Pkg.Netcore.Data.Contracts;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IProvideJobProfileCacheManagement :
        ICmsApiService
    {
        public Task<TModel> GetBuiltItemAsync<TModel, TChild>(Uri url)
            where TModel : class, IBaseContentItemModel<TChild>
            where TChild : class, IBaseContentItemModel<TChild>;
    }
}
