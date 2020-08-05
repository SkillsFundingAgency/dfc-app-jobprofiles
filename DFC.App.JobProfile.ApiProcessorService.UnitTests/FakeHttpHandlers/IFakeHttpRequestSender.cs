using System.Net.Http;

namespace DFC.App.JobProfile.ApiProcessorService.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
