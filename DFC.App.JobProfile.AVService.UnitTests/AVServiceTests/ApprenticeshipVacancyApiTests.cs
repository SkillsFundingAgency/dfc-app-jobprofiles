using DFC.App.JobProfile.Data.Configuration;
using DFC.App.JobProfile.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.AVService.UnitTests
{
    [Trait("Apprenticeship Vacancy Api", "Tests")]
    public class ApprenticeshipVacancyApiTests
    {
        private readonly ILogger<ApprenticeshipVacancyApi> fakeLogger;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;

        public ApprenticeshipVacancyApiTests()
        {
            fakeLogger = A.Fake<ILogger<ApprenticeshipVacancyApi>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100, FAAEndPoint = "https://doesnotgoanywhere.com", RequestTimeOutSeconds = 10 };
        }

        [Fact]
        public async Task GetAsyncTestAsync()
        {
            //arrange
            const string expectedResponse = "ExpectedResponse";

            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(expectedResponse) };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, aVAPIServiceSettings, httpClient);
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            //Act
            var result = await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.ListVacancies).ConfigureAwait(false);

            //Asserts
            Assert.Equal(expectedResponse, result);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        [Fact]
        public async Task GetAsyncWithErrorTest()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent("ExpectedResponse") };
            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            var apprenticeshipVacancyApi = new ApprenticeshipVacancyApi(fakeLogger, aVAPIServiceSettings, httpClient);
            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () => await apprenticeshipVacancyApi.GetAsync("fakeRequest", RequestType.ListVacancies).ConfigureAwait(false)).ConfigureAwait(false);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }
    }
}