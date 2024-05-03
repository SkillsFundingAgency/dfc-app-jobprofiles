using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetById Tests")]
    public class ProfileServiceGetByIdTests
    {
        private readonly ICosmosRepository<Data.Models.JobProfileModel> repository;

        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine fakeRazorTemplateEngine;
        private readonly IConfiguration fakeConfiguration;
        private readonly ICourseSearchApiService fakeclient;

        public ProfileServiceGetByIdTests()
        {
            repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            segmentService = A.Fake<ISegmentService>();
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            fakeConfiguration = A.Fake<IConfiguration>();
            fakeclient = A.Fake<ICourseSearchApiService>();
            jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeclient);
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
            Data.Models.JobProfileModel expectedResult = null;

            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).Returns(expectedResult);

            // act
            var result = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            // assert
            A.CallTo(() => repository.GetAsync(A<Expression<Func<JobProfileModel, bool>>>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, expectedResult);
        }
    }
}