using System.Net.Http;

namespace DFC.App.JobProfile.AVService.UnitTests
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}