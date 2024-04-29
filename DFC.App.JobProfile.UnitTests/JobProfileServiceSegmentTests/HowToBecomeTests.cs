using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ProfileService;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Common;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
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
    public class HowToBecomeTests
    {
        [Fact]
        public async Task GetHowToBecomeValidInputAsync()
        {
            //Arrange
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var segmentService = A.Fake<ISegmentService>();
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();

            var jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration);
            var expectedResult = GetExpectedData();

            var canonicalName = "bookmaker";
            var filter = "PUBLISHED";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);

            //Act
            var response = await jobProfileService.GetHowToBecomeSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetHowToBecomeInvalidInputAsync()
        {
            var repository = A.Fake<ICosmosRepository<JobProfileModel>>();
            var segmentService = A.Fake<ISegmentService>();
            var mapper = GetMapperInstance();

            var logService = A.Fake<ILogService>();
            var sharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var configuration = A.Fake<IConfiguration>();

            var jobProfileService = new JobProfileService(repository, segmentService, mapper, logService, sharedContentRedisInterface, razorTemplateEngine, configuration);
            var expectedResult = GetExpectedData();
            var canonicalName = "bookmaker";
            var filter = "PUBLISHED";

            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfileHowToBecomeResponse());

            //Act
            var response = await jobProfileService.GetHowToBecomeSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfileHowToBecomeResponse GetExpectedData()
        {
            var expectedResult = new JobProfileHowToBecomeResponse();

            var contentItemHTB = new ContentItemHTB
            {
                Body = new Body { Html = string.Empty},
                DisplayText = string.Empty,
                Description = string.Empty,
                FurtherInformation = new FurtherInformation { Html = string.Empty },
                Info = new Info { Html = string.Empty },
                Text = string.Empty,
                URL = string.Empty,
                Thumbnail = new Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Thumbnail { Paths = new List<string>(), Urls = new List<string>() },
            };

            var contentItemHTBList = new List<ContentItemHTB> { contentItemHTB };

            var list = new List<JobProfileHowToBecome>
            {
                new JobProfileHowToBecome
                {
                    DisplayText = "Bookmaker",
                    PageLocation = new PageLocation { FullUrl = "/bookmaker", UrlName = "bookmaker" },
                    EntryRoutes = new EntryRoutes { Html = string.Empty },
                    UniversityRelevantSubjects = new UniversityRelevantSubjects { Html = string.Empty },
                    UniversityFurtherRouteInfo = new UniversityFurtherRouteInfo { Html = string.Empty },
                    UniversityEntryRequirements = new UniversityEntryRequirements { ContentItems = contentItemHTBList },
                    RelatedUniversityRequirements = new RelatedUniversityRequirements { ContentItems = contentItemHTBList },
                    RelatedUniversityLinks = new RelatedUniversityLinks { ContentItems = contentItemHTBList },
                    CollegeRelevantSubjects = new CollegeRelevantSubjects { Html = string.Empty },
                    CollegeFurtherRouteInfo = new CollegeFurtherRouteInfo { Html = string.Empty },
                    CollegeEntryRequirements = new CollegeEntryRequirements { ContentItems = contentItemHTBList },
                    RelatedCollegeRequirements = new RelatedCollegeRequirements { ContentItems = contentItemHTBList },
                    RelatedCollegeLinks = new RelatedCollegeLinks { ContentItems = contentItemHTBList },
                    ApprenticeshipRelevantSubjects = new ApprenticeshipRelevantSubjects { Html = string.Empty },
                    ApprenticeshipFurtherRoutesInfo = new ApprenticeshipFurtherRoutesInfo { Html = string.Empty },
                    ApprenticeshipEntryRequirements = new ApprenticeshipEntryRequirements { ContentItems = contentItemHTBList },
                    RelatedApprenticeshipRequirements = new RelatedApprenticeshipRequirements { ContentItems = contentItemHTBList },
                    RelatedApprenticeshipLinks = new RelatedApprenticeshipLinks { ContentItems = contentItemHTBList },
                    Work = new Work { Html = string.Empty },
                    Volunteering = new Volunteering { Html = string.Empty },
                    DirectApplication = new DirectApplication { Html = string.Empty },
                    OtherRoutes = new Otherroutes { Html = string.Empty },
                    ProfessionalAndIndustryBodies = new ProfessionalAndIndustryBodies { Html = string.Empty },
                    FurtherInformation = new FurtherInformation { Html = string.Empty },
                    CareerTips = new CareerTips { Html = string.Empty },
                    RelatedRegistrations = new RelatedRegistrations { ContentItems = contentItemHTBList },
                    RealStory = new RealStory { ContentItems = contentItemHTBList },
                },
            };

            expectedResult.JobProfileHowToBecome = list;
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
