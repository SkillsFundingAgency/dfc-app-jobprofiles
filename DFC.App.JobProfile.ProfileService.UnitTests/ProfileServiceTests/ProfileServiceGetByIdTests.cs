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
    [Trait("Profile Service", "GetById Tests")]
    public class ProfileServiceGetByIdTests
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly IDraftJobProfileService draftJobProfileService;
        private readonly IJobProfileService jobProfileService;

        public ProfileServiceGetByIdTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            draftJobProfileService = A.Fake<DraftJobProfileService>();
            jobProfileService = new JobProfileService(repository, draftJobProfileService);
        }

        [Fact]
        public async Task JobProfileServiceGetByIdReturnsSuccess()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            var expectedResult = A.Fake<JobProfileModel>();

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }

        [Fact]
        public async Task JobProfileServiceGetByIdReturnsNullWhenMissingInRepository()
        {
            // arrange
            Guid documentId = Guid.NewGuid();
            JobProfileModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}