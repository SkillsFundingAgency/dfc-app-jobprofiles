using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetAll Tests")]
    public class ProfileServiceGetAllTests
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceGetAllTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<DraftJobProfileService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService);
        }

        [Fact]
        public async Task JobProfileServiceGetAllListReturnsSuccess()
        {
            // arrange
            var expectedResults = A.CollectionOfFake<JobProfileModel>(2);

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await jobProfileService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }

        [Fact]
        public async Task JobProfileServiceGetAllListReturnsNullWhenMissingRepository()
        {
            // arrange
            IEnumerable<JobProfileModel> expectedResults = null;

            A.CallTo(() => repository.GetAllAsync()).Returns(expectedResults);

            // act
            var results = await jobProfileService.GetAllAsync().ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAllAsync()).MustHaveHappenedOnceExactly();
            A.Equals(results, expectedResults);
        }
    }
}
