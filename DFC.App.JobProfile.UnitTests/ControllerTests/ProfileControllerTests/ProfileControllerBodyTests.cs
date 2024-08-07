﻿using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using DFC.App.JobProfile.Data.Enums;
using Xunit;
using DFC.App.JobProfile.Exceptions;

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

            expectedResult.CanonicalName = FakeArticleName;

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(DefaultBodyViewModel);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task JobProfileControllerBodyJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Dummy<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.CanonicalName = FakeArticleName;
            expectedResult.Segments = new List<SegmentModel>();

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(DefaultBodyViewModel);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<List<SegmentModel>>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task JobProfileControllerBodyHtmlAndJsonReturnsNoContentWhenNoAlternateArticle(
            string mediaTypeName)
        {
            // Arrange
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.IsType<NotFoundResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task JobProfileControllerBodyHtmlAndJsonReturnsBadRequestWhenHostIsInvalid (string mediaTypeName)
        {
            // Arrange
            var controller = BuildProfileController(mediaTypeName, host: "notlocalhost");

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustNotHaveHappened();
            Assert.IsType<BadRequestObjectResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task JobProfileControllerBodyReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            expectedResult.CanonicalName = FakeArticleName;

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(DefaultBodyViewModel);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task BodyThrowsInvalidProfileExceptionWhenCriticalSegmentDoesNotExist()
        {
            // Arrange
            var controller = BuildProfileController(MediaTypeNames.Application.Json);
            var bodyViewModel = new BodyViewModel
            {
                CanonicalName = FakeArticleName,
                Segments = new List<SegmentModel>
                {
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

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(A.Fake<JobProfileModel>());
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(bodyViewModel);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            await Assert.ThrowsAsync<InvalidProfileException>(async () => await controller.Body(FakeArticleName).ConfigureAwait(false)).ConfigureAwait(false);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(EmptyCriticalSegmentModelInput))]
        public async Task BodyThrowsInvalidProfileExceptionWhenCriticalSegmentsHaveNoMarkup(List<SegmentModel> segments)
        {
            // Arrange
            var controller = BuildProfileController(MediaTypeNames.Application.Json);
            var bodyViewModel = new BodyViewModel
            {
                CanonicalName = FakeArticleName,
                Segments = segments,
            };

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(A.Fake<JobProfileModel>());
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(bodyViewModel);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            await Assert.ThrowsAsync<InvalidProfileException>(async () => await controller.Body(FakeArticleName).ConfigureAwait(false)).ConfigureAwait(false);

            controller.Dispose();
        }
    }
}