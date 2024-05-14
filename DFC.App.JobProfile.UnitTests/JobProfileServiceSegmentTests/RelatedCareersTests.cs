using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.JobProfileServiceSegmentTests
{
    public class RelatedCareersTests
    {
        [Fact]
        public async Task GetRelatedCareersDataSuccess()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();

            var canonicalName = "biochemist";
            var filter = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakefacclient, fakeAVAPIService);
            var expectedResult = GetExpectedData();

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetRelatedCareersSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetRelatedCareersDataNoSuccessAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();

            var canonicalName = "biochemist";
            var filter = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakefacclient, fakeAVAPIService);

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new RelatedCareersResponse());

            //Act
            var response = await jobProfileService.GetRelatedCareersSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static RelatedCareersResponse GetExpectedData()
        {
            var expectedResult = new RelatedCareersResponse();

            var list = new List<JobProfileRelatedCareers>
           {
               new JobProfileRelatedCareers
               {
                   RelatedCareerProfiles = new RelatedCareers
                   {
                       ContentItems = new List<RelatedCareersContentItems>
                       {
                           new RelatedCareersContentItems()
                           {
                               DisplayText = "LabTech",
                               ContentItemId = "12345",
                               GraphSync = new ()
                               {
                                   NodeId = "123",
                               },
                               PageLocation = new ()
                               {
                                   DefaultPageForLocation = true,
                                   FullUrl = "/labtech",
                                   UrlName = "labtech",
                               },
                           },
                       },
                   },
                   DisplayText = "biochemist",
                   PageLocation = new ()
                   {
                       DefaultPageForLocation = false,
                       FullUrl = "/biochemist",
                       UrlName = "biochemist",
                   },
               },
           };
            expectedResult.JobProfileRelatedCareers = list;
            return expectedResult;
        }

        private IMapper GetMapperInstance()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new JobProfileModelProfile());
            });
            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}
