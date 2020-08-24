using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using FakeItEasy;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "Ping / Health Tests")]
    public class ProfileServicePingTests
    {
        private IMapper mapper;

        [Fact]
        public void JobProfileServicePingReturnsSuccess()
        {
            // arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var expectedResult = true;
            mapper = A.Fake<IMapper>();

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var jobProfileService = new JobProfileService(repository, mapper);

            // act
            var result = jobProfileService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public void JobProfileServicePingReturnsFalseWhenMissingRepository()
        {
            // arrange
            var repository = A.Dummy<ICosmosRepository<JobProfileModel>>();
            var expectedResult = false;

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            var jobProfileService = new JobProfileService(repository, mapper);

            // act
            var result = jobProfileService.PingAsync().Result;

            // assert
            A.CallTo(() => repository.PingAsync()).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}