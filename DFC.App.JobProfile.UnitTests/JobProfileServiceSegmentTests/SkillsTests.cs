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
    public class SkillsTests
    {
        [Fact]
        public async Task GetSkillsDataSuccessAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeCourseSearch = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var canonicalName = "biochemist";
            var filter = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakeCourseSearch, fakeAVAPIService);
            var expectedResult = GetExpectedData();
            var expectedSkillsResult = GetSkillsData();

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedResult);
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(expectedSkillsResult);

            //Act
            var response = await jobProfileService.GetSkillSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.NotNull(response);
            response.Should().BeOfType(typeof(SegmentModel));
        }

        [Fact]
        public async Task GetSkillsDataNoSuccessAsync()
        {
            //Arrange
            var mapper = GetMapperInstance();
            var logService = A.Fake<ILogService>();
            var fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            var configuration = A.Fake<IConfiguration>();
            var razorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            var fakeCourseSearch = A.Fake<ICourseSearchApiService>();
            var fakeAVAPIService = A.Fake<IAVAPIService>();

            var canonicalName = "biochemist";
            var filter = "PUBLISHED";

            var jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, razorTemplateEngine, configuration, fakeCourseSearch, fakeAVAPIService);

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new JobProfileSkillsResponse());
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).Returns(new SkillsResponse());

            //Act
            var response = await jobProfileService.GetSkillSegmentAsync(canonicalName, filter);

            //Assert
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored)).MustHaveHappenedOnceExactly();
            response.Should().BeOfType(typeof(SegmentModel));
        }

        private static JobProfileSkillsResponse GetExpectedData()
        {
            var expectedResult = new JobProfileSkillsResponse();
            var list = new List<JobProfileSkill>()
            {
                new JobProfileSkill
                {
                    DigitalSkills = new DigitalSkills
                    {
                        ContentItems = new List<DigitalSkillsContentItem>
                        {
                            new DigitalSkillsContentItem()
                            {
                                Description = "Skill1Desc",
                                DisplayText = "Skill1",
                                GraphSync = new ()
                                {
                                    NodeId = "29d1a617-92b7-446f-81a1-070e69b00aa9",
                                },
                            },
                        },
                    },
                    DisplayText = "BioSkill",
                    Otherrequirements = new ()
                   {
                       Html = "ExampleHTML",
                   },
                    PageLocation = new ()
                   {
                       DefaultPageForLocation = false,
                       FullUrl = "/biochemist",
                       UrlName = "biochemist",
                   },
                    Relatedrestrictions = new Relatedrestrictions
                   {
                       ContentItems = new List<RelatedrestrictionsContentItem>
                       {
                           new RelatedrestrictionsContentItem()
                           {
                               DisplayText = "Restriction1",
                               GraphSync = new ()
                               {
                                   NodeId = "29d1a617-92b7-446f-81a1-070e69b00aa9",
                               },
                               Info = new ()
                               {
                                   Html = "InfoExampleText",
                               },
                           },
                       },
                   },
                    Relatedskills = new Relatedskills
                   {
                       ContentItems = new List<RelatedSkill>
                       {
                           new RelatedSkill()
                           {
                               DisplayText = "RelatedSkill1",
                               RelatedSkillDesc = "RelatedSkillDesc",
                               GraphSync = new ()
                               {
                                   NodeId = "29d1a617-92b7-446f-81a1-070e69b00aa9",
                               },
                               ONetAttributeType = "testONet",
                               ONetRank = "3.185",
                               Ordinal = 1,
                               RelatedSOCcode = "TestSocCode",
                           },
                       },
                   },
                },
            };
            expectedResult.JobProfileSkills = list;
            return expectedResult;
        }

        private static SkillsResponse GetSkillsData()
        {
            var expectedResult = new SkillsResponse();

            var list = new List<Skills>
            {
                new Skills
                {
                    Description = "Skill1Desc",
                    DisplayText = "RelatedSkillDesc",
                    GraphSync = new()
                    {
                        NodeId = "29d1a617-92b7-446f-81a1-070e69b00aa9",
                    },
                    ONetElementId = "12345",
                },
            };
            expectedResult.Skill = list;
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
