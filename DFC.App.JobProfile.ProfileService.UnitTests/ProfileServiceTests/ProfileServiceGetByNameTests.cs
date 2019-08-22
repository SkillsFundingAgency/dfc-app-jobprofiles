using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetByName Tests")]
    public class ProfileServiceGetByNameTests
    {
        private readonly ICosmosRepository<JobProfileModel> Repository;
        private readonly IDraftJobProfileService DraftJobProfileService;
        private readonly IJobProfileService JobProfileService;

        public ProfileServiceGetByNameTests()
        {
            Repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            DraftJobProfileService = A.Fake<DraftJobProfileService>();
            JobProfileService = new JobProfileService(Repository, DraftJobProfileService);
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => Repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await JobProfileService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => Repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await JobProfileService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: canonicalName", exceptionResult.Message);
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsNullWhenMissingInRepository()
        {
            // arrange
            JobProfileModel expectedResult = null;

            A.CallTo(() => Repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await JobProfileService.GetByNameAsync("article-name").ConfigureAwait(false);

            // assert
            A.CallTo(() => Repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}