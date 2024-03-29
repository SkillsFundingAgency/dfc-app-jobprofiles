﻿using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Exceptions;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
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
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task JobProfileControllerBodyJsonAndHtmlReturnsRedirectWhenAlternateArticleExists(
            string mediaTypeName)
        {
            // Arrange
            var expectedAlternativeResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .Returns(expectedAlternativeResult);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<RedirectResult>(result);
            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            Assert.True(statusResult.Permanent);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task JobProfileControllerBodyJsonAndHtmlReturnsRedirectWhenAlternateArticleExistsWithValidHost(string mediaTypeName)
        {
            // Arrange
            var expectedAlternativeResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName, host: "localhosttest", whitelist: new string[] { "6357b4c93a1bfd4901871836a03602586222c6a9ae05a48d67dda806d2c2eca6" });

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .Returns(expectedAlternativeResult);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);
            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<RedirectResult>(result);
            statusResult.Url.Should().NotBeNullOrWhiteSpace();
            Assert.True(statusResult.Permanent);

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
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .Returns((JobProfileModel)null);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
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
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored)).MustNotHaveHappened();
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
        public async Task BodyReturnsInvalidMessageWhenCriticalSegmentDoesNotExist()
        {
            // Arrange
            var controller = BuildProfileController(MediaTypeNames.Application.Json);
            var expectedOutput = "<header class='job-profile-hero'> <div class='govuk-width-container'> <div class='govuk-breadcrumbs'> <ol class='govuk-breadcrumbs__list'> <li class='govuk-breadcrumbs__list-item'> <a class='govuk-breadcrumbs__link' href='/explore-careers'>Home: Explore careers</a> </li> </ol> </div> <div class='govuk-grid-row'> <div class='govuk-grid-column-two-thirds'> <p>We are aware there is a problem with this profile and we are working hard to fix it</p> </div> </div </div> </header>";
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
            A.CallTo(() => FakeJobProfileService.GetByAlternativeNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);
            A.CallTo(() => FakeMapper.Map<BodyViewModel>(A<JobProfileModel>.Ignored)).Returns(bodyViewModel);
            A.CallTo(() => FakeRedirectionSecurityService.IsValidHost(A<Uri>.Ignored)).Returns(true);

            // Act
            var result = await controller.Body(FakeArticleName).ConfigureAwait(false);
            var profResult = (List<SegmentModel>)result.As<OkObjectResult>().Value;
            Assert.Equal(expectedOutput, profResult[0].Markup.Value);
            controller.Dispose();
        }
    }
}