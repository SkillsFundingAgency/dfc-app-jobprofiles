using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.DraftProfileService;
using FakeItEasy;
using System;
using System.Linq.Expressions;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetByName Tests")]
    public class ProfileServiceGetByAlternativeNameTests
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly SegmentService segmentService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceGetByAlternativeNameTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<DraftJobProfileService>();
            segmentService = A.Fake<SegmentService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService);
        }

        [Fact]
        public void HelpPageServiceGetByAlternativeNameReturnsSuccess()
        {
            // arrange
            const string alternativeName = "name1";
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.GetByAlternativeNameAsync(alternativeName).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async System.Threading.Tasks.Task HelpPageServiceGetByAlternativeNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.GetByAlternativeNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: alternativeName", exceptionResult.Message);
        }

        [Fact]
        public void HelpPageServiceGetByAlternativeNameReturnsNullWhenMissingRepository()
        {
            // arrange
            const string alternativeName = "name1";
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.GetByAlternativeNameAsync(alternativeName).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
