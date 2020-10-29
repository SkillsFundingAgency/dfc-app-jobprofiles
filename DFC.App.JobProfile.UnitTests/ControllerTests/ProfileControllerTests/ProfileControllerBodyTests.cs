using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Exceptions;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Body Tests")]
    public class ProfileControllerBodyTests : BaseProfileController
    {
        private const string FakeArticleName = "an-article-name";

        private static BodyViewModel DefaultBodyViewModel => new BodyViewModel
        {
            CanonicalName = FakeArticleName,
            Segments = new List<SegmentModel>
            {
                new SegmentModel
                {
                    Segment = JobProfileSegment.Overview,
                    Markup = new HtmlString("someContent"),
                },
                new SegmentModel
                {
                    Segment = JobProfileSegment.WhatItTakes,
                    Markup = new HtmlString("someContent"),
                },
                new SegmentModel
                {
                    Segment = JobProfileSegment.HowToBecome,
                    Markup = new HtmlString("someContent"),
                },
            },
        };


        private static DocumentViewModel DocumentBodyViewModel => new DocumentViewModel
        {
            JobProfileWebsiteUrl = FakeArticleName
        };


        public static IEnumerable<object[]> EmptyCriticalSegmentModelInput()
        {
            yield return new object[]
            {
                new List<SegmentModel>
                {
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.Overview,
                        Markup = new HtmlString(string.Empty),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.HowToBecome,
                        Markup = new HtmlString("someContent"),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.WhatItTakes,
                        Markup = new HtmlString("someContent"),
                    },
                },
            };
            yield return new object[]
            {
                new List<SegmentModel>
                {
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.Overview,
                        Markup = new HtmlString("someContent"),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.HowToBecome,
                        Markup = new HtmlString(string.Empty),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.WhatItTakes,
                        Markup = new HtmlString("someContent"),
                    },
                },
            };
            yield return new object[]
            {
                new List<SegmentModel>
                {
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.Overview,
                        Markup = new HtmlString("someContent"),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.HowToBecome,
                        Markup = new HtmlString("someContent"),
                    },
                    new SegmentModel
                    {
                        Segment = JobProfileSegment.WhatItTakes,
                        Markup = new HtmlString(string.Empty),
                    },
                },
            };
        }

        public static IEnumerable<object[]> EmptyNonCriticalSegmentModelInput()
        {
            yield return new object[]
            {
                DefaultBodyViewModel.Segments.Append(new SegmentModel
                {
                    Segment = JobProfileSegment.RelatedCareers,
                    Markup = new HtmlString(string.Empty),
                }).ToList(),
            };
            yield return new object[]
            {
                DefaultBodyViewModel.Segments.Append(new SegmentModel
                {
                    Segment = JobProfileSegment.CurrentOpportunities,
                    Markup = new HtmlString(string.Empty),
                }).ToList(),
            };
            yield return new object[]
            {
                DefaultBodyViewModel.Segments.Append(new SegmentModel
                {
                    Segment = JobProfileSegment.WhatYouWillDo,
                    Markup = new HtmlString(string.Empty),
                }).ToList(),
            };
            yield return new object[]
            {
                DefaultBodyViewModel.Segments.Append(new SegmentModel
                {
                    Segment = JobProfileSegment.CareerPathsAndProgression,
                    Markup = new HtmlString(string.Empty),
                }).ToList(),
            };
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task JobProfileControllerBodyHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.JobProfileWebsiteUrl = FakeArticleName;

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(DocumentBodyViewModel);

            // Act
            var result = await controller.Document(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

    }
}