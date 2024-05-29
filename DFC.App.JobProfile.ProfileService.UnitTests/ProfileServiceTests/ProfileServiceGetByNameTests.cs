using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.App.JobProfile.Data.Enums;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using Xunit;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Data.Models.Segment.SkillsModels;
using RelatedSkill = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.RelatedSkill;
using Relatedskills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Relatedskills;
using Skills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Skills;
using DFC.App.JobProfile.Data.Models.Segment.Overview;

namespace DFC.App.JobProfile.ProfileService.UnitTests.ProfileServiceTests
{
    [Trait("Profile Service", "GetByName Tests")]
    public class ProfileServiceGetByNameTests
    {
        private readonly IMapper mapper;
        private readonly IJobProfileService jobProfileService;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface fakeSharedContentRedisInterface;
        private readonly IRazorTemplateEngine fakeRazorTemplateEngine;
        private readonly IConfiguration fakeConfiguration;
        private readonly IAVAPIService fakeAVAPIService;
        private readonly ICourseSearchApiService fakeFACClient;

        public ProfileServiceGetByNameTests()
        {
            mapper = A.Fake<IMapper>();
            logService = A.Fake<ILogService>();
            fakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            fakeRazorTemplateEngine = A.Fake<IRazorTemplateEngine>();
            fakeConfiguration = A.Fake<IConfiguration>();
            fakeAVAPIService = A.Fake<IAVAPIService>();
            fakeFACClient = A.Fake<ICourseSearchApiService>();
            jobProfileService = new JobProfileService(mapper, logService, fakeSharedContentRedisInterface, fakeRazorTemplateEngine, fakeConfiguration, fakeFACClient, fakeAVAPIService);
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsSuccess()
        {
            // arrange
            var expectedResult = A.Fake<JobProfileModel>();
            expectedResult.Segments = new List<SegmentModel>
            {
                new SegmentModel { Segment = JobProfileSegment.Overview },
                new SegmentModel { Segment = JobProfileSegment.CurrentOpportunities },
                new SegmentModel { Segment = JobProfileSegment.RelatedCareers },
                new SegmentModel { Segment = JobProfileSegment.HowToBecome },
                new SegmentModel { Segment = JobProfileSegment.CareerPathsAndProgression },
                new SegmentModel { Segment = JobProfileSegment.WhatItTakes },
                new SegmentModel { Segment = JobProfileSegment.WhatYouWillDo },
            };

            var expectedSkillsResponse = new SkillsResponse();
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
            expectedSkillsResponse.Skill = list;

            var allEnumValues = Enum.GetValues(typeof(JobProfileSegment)).Cast<JobProfileSegment>();
            var distinctSegments = expectedResult.Segments.Select(s => s.Segment).Distinct();

            // Fake Skills GraphQl returns
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(new JobProfilesOverviewResponse()
                {
                    JobProfileOverview = new List<JobProfileOverview> { new JobProfileOverview() },
                });
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(new JobProfileHowToBecomeResponse()
                {
                    JobProfileHowToBecome = new List<JobProfileHowToBecome> { new JobProfileHowToBecome() },
                });

            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(ExpectedJobProfileSkillsResponse());
            A.CallTo(() => fakeSharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(A<string>.Ignored, A<string>.Ignored, A<double>.Ignored))
                .Returns(expectedSkillsResponse);

            // Fake AutoMapper returns
            A.CallTo(() => fakeRazorTemplateEngine.RenderAsync(A<string>.Ignored, A<OverviewApiModel>.Ignored, null))
                .Returns("test");
            A.CallTo(() => fakeRazorTemplateEngine.RenderAsync(A<string>.Ignored, A<HowToBecomeSegmentDataModel>.Ignored, null))
                .Returns("test");
            A.CallTo(() => fakeRazorTemplateEngine.RenderAsync(A<string>.Ignored, A<JobProfileSkillSegmentDataModel>.Ignored, null))
                .Returns("test");

            // Fake Skills AutoMapper returns
            A.CallTo(() => mapper.Map<List<OnetSkill>>(A<List<Skills>>.Ignored))
                .Returns(new List<OnetSkill>() { new OnetSkill() { Title = "RelatedSkillDesc" } });

            A.CallTo(() => mapper.Map<List<ContextualisedSkill>>(A<List<RelatedSkill>>.Ignored))
                .Returns(new List<ContextualisedSkill>() { new ContextualisedSkill() { Description = "RelatedSkillDesc" } });


            // act
            var result = await jobProfileService.GetByNameAsync("auditor").ConfigureAwait(false);

            // assert
            Assert.Equal(result.Segments.Count, expectedResult.Segments.Count);
            Assert.Equal(allEnumValues.Count(), distinctSegments.Count());
        }

        [Fact]
        public async Task JobProfileServiceGetByNameReturnsArgumentNullExceptionWhenNullIsUsed()
        {
            // arrange

            // act
            var exceptionResult = await Assert.ThrowsAsync<ArgumentNullException>(async () => await jobProfileService.GetByNameAsync(null).ConfigureAwait(false)).ConfigureAwait(false);

            // assert
            Assert.Equal("Value cannot be null. (Parameter 'canonicalName')", exceptionResult.Message);
        }

        private static JobProfileSkillsResponse ExpectedJobProfileSkillsResponse()
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
    }
}