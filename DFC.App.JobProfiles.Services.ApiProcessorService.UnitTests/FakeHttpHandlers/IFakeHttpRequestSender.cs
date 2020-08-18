using System;
using System.Net.Http;

namespace DFC.App.JobProfile.Services.ApiProcessorService.UnitTests.FakeHttpHandlers
{
    public interface IFakeHttpRequestSender
    {
        HttpResponseMessage Send(HttpRequestMessage request);
    }
}
