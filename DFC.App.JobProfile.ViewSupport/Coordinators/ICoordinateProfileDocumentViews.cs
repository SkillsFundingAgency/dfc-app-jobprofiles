using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Coordindators
{
    public interface ICoordinateProfileDocumentViews
    {
        Task<HttpResponseMessage> GetSummaryDocuments();

        Task<HttpResponseMessage> GetDocumentFor(string occupationName);
    }
}
