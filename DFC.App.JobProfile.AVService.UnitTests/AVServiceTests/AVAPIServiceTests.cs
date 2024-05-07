using DFC.App.JobProfile.Data.Configuration;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.AVService.UnitTests
{
    [Trait("AVAPI Service", "Tests")]
    public class AVAPIServiceTests
    {
        private readonly ILogger<AVAPIService> fakeLogger;
        private readonly AVAPIServiceSettings aVAPIServiceSettings;
        private readonly IApprenticeshipVacancyApi fakeApprenticeshipVacancyApi;
        private readonly AVMapping aVMapping;

        public AVAPIServiceTests()
        {
            fakeLogger = A.Fake<ILogger<AVAPIService>>();
            aVAPIServiceSettings = new AVAPIServiceSettings() { FAAMaxPagesToTryPerMapping = 100 };
            fakeApprenticeshipVacancyApi = A.Fake<IApprenticeshipVacancyApi>();
            aVMapping = new AVMapping
            {
                Standards = new string[] { "225" },
                Frameworks = new string[] { "512" },
            };
        }

        [Fact]
        public async Task GetAVSumaryPageTestAsync()
        {
            //arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 1;
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.ListVacancies)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, 50, pageSize, pageSize, returnDiffrentProvidersOnPage));
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var pageSumary = await aVAPIService.GetAVSummaryPageAsync(aVMapping, 1).ConfigureAwait(false);

            //Asserts
            pageSumary.Vacancies.Count().Should().Be(pageSize);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.ListVacancies)).MustHaveHappened();
        }

        [Fact]
        public void GetAVSumaryPageAsyncNullExceptionsTest()
        {
            //Setup
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Asserts
            Func<Task> f = async () => { await aVAPIService.GetAVSummaryPageAsync(null, 1).ConfigureAwait(false); };
            f.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAVsForMultipleProvidersTestAsync()
        {
            //Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var returnDiffrentProvidersOnPage = 2;

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.ListVacancies)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber, 50, pageSize, pageSize, returnDiffrentProvidersOnPage)).Once().
                Then.Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancySummaryResponse(pageNumber + 1, 50, pageSize, pageSize, returnDiffrentProvidersOnPage));

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var aVSumaryList = await aVAPIService.GetAVsForMultipleProvidersAsync(aVMapping).ConfigureAwait(false);

            //Asserts
            //must have got more then 1 page to get multiple supplier
            aVSumaryList.Count().Should().BeGreaterThan(pageSize);

            var numberProviders = aVSumaryList.Select(v => v.ProviderName).Distinct().Count();
            numberProviders.Should().BeGreaterThan(1);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.ListVacancies)).MustHaveHappenedTwiceExactly();
        }

        [Fact]
        public void GetAVsForMultipleProvidersAsyncNullExceptionsTest()
        {
            //Setup
            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Asserts
            Func<Task> f = async () => { await aVAPIService.GetAVsForMultipleProvidersAsync(null).ConfigureAwait(false); };
            f.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetApprenticeshipVacancyDetailsTestAsync()
        {
            //Arrange
            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.VacancyByReference)).Returns(AVAPIDummyResponses.GetDummyApprenticeshipVacancyDetailsResponse());

            var aVAPIService = new AVAPIService(fakeApprenticeshipVacancyApi, fakeLogger, aVAPIServiceSettings);

            //Act
            var vacancyDetails = await aVAPIService.GetApprenticeshipVacancyDetailsAsync(123).ConfigureAwait(false);

            //Asserts
            vacancyDetails.Should().NotBeNull();
            vacancyDetails.VacancyReference.Should().Be(123);

            A.CallTo(() => fakeApprenticeshipVacancyApi.GetAsync(A<string>._, RequestType.VacancyByReference)).MustHaveHappened();
        }
    }
}
