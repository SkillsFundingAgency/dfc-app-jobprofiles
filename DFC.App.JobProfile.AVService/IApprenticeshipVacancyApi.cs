using System.Threading.Tasks;

namespace DFC.App.JobProfile.AVService
{
    public enum RequestType
    {
        ListVacancies,
        VacancyByReference,
    }

    public interface IApprenticeshipVacancyApi
    {
        Task<string> GetAsync(string requestQueryString, RequestType requestType);
    }
}