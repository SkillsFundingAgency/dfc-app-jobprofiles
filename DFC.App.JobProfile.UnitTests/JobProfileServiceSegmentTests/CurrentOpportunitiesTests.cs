using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
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
    public class CurrentOpportunitiesTests
    {
        private const string CanonicalName = "auditor";

        [Fact]
        public async Task GetCurrentOpportunitiesValidInputAsync()
        {
            //Arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var segmentService = A.Fake<ISegmentService>();
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeConfiguration = A.Fake<IConfiguration>();
            var fakefacclient = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, sharedContentRedisInterface, razorTemplateEngine, fakeConfiguration, fakefacclient, fakeAVAPIService);
            var expectedResult = GetExpectedData();

            var canonicalName = "auditor";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetCurrentOpportunities(canonicalName, "PUBLISHED");

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfileCurrentOpportunitiesGetbyUrlReponse GetExpectedData()
        {
            var expectedResult = new JobProfileCurrentOpportunitiesGetbyUrlReponse();
            var list = new List<JobProfileCurrentOpportunities>
            {
                new JobProfileCurrentOpportunities {
                    DisplayText = "Auditor",
                    Coursekeywords = "'building services engineering'",
                    SOCCode = new SOCCode()
                    {
                        ContentItems = new SOCCodeContentItem[]
                        {
                            new SOCCodeContentItem()
                            {
                                ApprenticeshipStandards = new ApprenticeshipStandards()
                                {
                                    ContentItems= new ApprenticeshipStandardsContentItem[]
                                    {
                                        new ApprenticeshipStandardsContentItem()
                                        {
                                            LARScode = "123456",
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            };

            expectedResult.JobProileCurrentOpportunitiesGetbyUrl = list;
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
