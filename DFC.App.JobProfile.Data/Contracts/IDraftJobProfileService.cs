using DFC.App.JobProfile.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IDraftJobProfileService
    {
        Task<JobProfileModel> GetSitefinityData(string canonicalName);
    }
}
