using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Coordindators
{
    public interface ICoordinateProfileDocumentViews
    {
        Task<HttpResponseMessage> GetSummaryDocuments();

        Task<HttpResponseMessage> GetDocumentFor(string occupationName, string address);

        Task<HttpResponseMessage> GetHeadFor(string occupationName, string address);

        Task<HttpResponseMessage> GetHeroBannerFor(string occupationName, string address);

        Task<HttpResponseMessage> GetBodyFor(Guid occupationID);
    }
}
