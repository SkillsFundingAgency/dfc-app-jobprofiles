using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.DraftProfileService
{
    public class DraftJobProfileService : IDraftJobProfileService
    {
        public Task<JobProfileModel> GetSitefinityData(string canonicalName)
        {
            throw new System.NotImplementedException();
        }
    }
}
