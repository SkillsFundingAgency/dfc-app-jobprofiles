using DFC.App.JobProfile.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Providers
{
    public interface IProvideSharedContent :
        IProvidePageContent<StaticItemCached>
    {
        Task<IReadOnlyCollection<StaticItemCached>> GetAllItems();
    }
}