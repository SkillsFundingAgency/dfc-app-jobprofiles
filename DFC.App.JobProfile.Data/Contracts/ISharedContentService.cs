using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface ISharedContentService
    {
        Task<Models.StaticContentItemModel> GetByNameAsync(string canonicalName);

        Task<List<Models.StaticContentItemModel>> GetByNamesAsync(List<string> contentList);
    }
}
