using AutoMapper;
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
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceGetByAlternativeNameTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<IDraftJobProfileService>();
            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService, segmentService, mapper);
        }

        [Fact]
        public void ProfileServiceGetByAlternativeNameReturnsSuccess()
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
        public async System.Threading.Tasks.Task ProfileServiceGetByAlternativeNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.GetByAlternativeNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null.\r\nParameter name: alternativeName", exceptionResult.Message);
        }

        [Fact]
        public void ProfileServiceGetByAlternativeNameReturnsNullWhenMissingRepository()
        {
            // arrange
            const string alternativeName = "name1";
            Data.Models.JobProfileModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = jobProfileService.GetByAlternativeNameAsync(alternativeName).Result;

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}
