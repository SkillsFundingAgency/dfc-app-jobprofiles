using AutoMapper;
using DFC.App.JobProfile.AutoMapperProfiles;
using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.ProfileControllerTests
{
    [Trait("Profile Controller", "Document Tests")]
    public class ProfileControllerDocumentTests : BaseProfileController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task ProfileControllerDocumentHtmlReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerDocumentJsonReturnsSuccess(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsAssignableFrom<DocumentViewModel>(jsonResult.Value);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        [MemberData(nameof(JsonMediaTypes))]
        public async Task ProfileControllerDocumentReturnsNoContentWhenNoData(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var controller = BuildProfileController(mediaTypeName);
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns((JobProfileModel)null);

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<NotFoundResult>(result);
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal((int)HttpStatusCode.NotFound, statusResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidMediaTypes))]
        public async Task ProfileControllerDocumentReturnsNotAcceptable(string mediaTypeName)
        {
            // Arrange
            const string article = "an-article-name";
            var expectedResult = A.Fake<JobProfileModel>();
            var controller = BuildProfileController(mediaTypeName);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(expectedResult);
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).Returns(A.Fake<DocumentViewModel>());

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => FakeMapper.Map<DocumentViewModel>(A<JobProfileModel>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal((int)HttpStatusCode.NotAcceptable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task ProfileControllerDocumentReturnsWithCorrectlyMappedHeadAndBodyContent()
        {
            // Arrange
            const string article = "an-article-name";
            const string headDescription = "HeadDescription";
            const string headTitle = "HeadTitle";
            const string headKeywords = "some keywords";

            var controller = BuildControllerWithMapper();
            var jobProfileModel = CreateJobProfileModel(headTitle, headDescription, headKeywords);
            var expectedViewModel = CreateDocumentViewModel(headTitle, headDescription, headKeywords);

            A.CallTo(() => FakeJobProfileService.GetByNameAsync(A<string>.Ignored)).Returns(jobProfileModel);

            // Act
            var result = await controller.Document(article).ConfigureAwait(false);

            // Assert
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var resultModel = Assert.IsAssignableFrom<DocumentViewModel>(jsonResult.Value);
            Assert.NotNull(resultModel.Head);
            Assert.NotNull(resultModel.Body);
            resultModel.Should().BeEquivalentTo(expectedViewModel);

            controller.Dispose();
        }

        private static JobProfileModel CreateJobProfileModel(string headTitle, string headDescription, string headKeywords)
        {
            return new JobProfileModel
            {
                MetaTags = new MetaTags
                {
                    Title = headTitle,
                    Description = headDescription,
                    Keywords = headKeywords,
                },
                Segments = new List<SegmentModel>
                {
                    new SegmentModel
                    {
                        Markup = new HtmlString("some markup"),
                        Segment = JobProfileSegment.HowToBecome,
                    },
                    new SegmentModel
                    {
                        Markup = new HtmlString("some markup"),
                        Segment = JobProfileSegment.Overview,
                    },
                },
                AlternativeNames = new List<string>(),
            };
        }

        private static DocumentViewModel CreateDocumentViewModel(string headTitle, string headDescription, string headKeywords)
        {
            return new DocumentViewModel
            {
                Head = new HeadViewModel
                {
                    Title = $"{headTitle} | Explore careers | National Careers Service",
                    Description = headDescription,
                    Keywords = headKeywords,
                },
                Description = headDescription,
                Keywords = headKeywords,
                Title = headTitle,
                AlternativeNames = Array.Empty<string>(),
                Body = new BodyViewModel
                {
                    Segments = new List<SegmentModel>
                    {
                        new SegmentModel
                        {
                            Markup = new HtmlString("some markup"),
                            Segment = JobProfileSegment.HowToBecome,
                        },
                        new SegmentModel
                        {
                            Markup = new HtmlString("some markup"),
                            Segment = JobProfileSegment.Overview,
                        },
                    },
                },
                Breadcrumb = new BreadcrumbViewModel
                {
                    Paths = new List<BreadcrumbPathViewModel>
                    {
                        new BreadcrumbPathViewModel
                        {
                            AddHyperlink = true,
                            Route = "/",
                            Title = "Home",
                        },
                        new BreadcrumbPathViewModel
                        {
                            AddHyperlink = true,
                            Route = "/job-profiles",
                            Title = "Job Profiles",
                        },
                        new BreadcrumbPathViewModel
                        {
                            AddHyperlink = false,
                            Route = "/job-profiles/",
                            Title = null,
                        },
                    },
                },
            };
        }

        private ProfileController BuildControllerWithMapper()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new JobProfileModelProfile());
            });

            var mapper = new Mapper(config);

            return BuildProfileController(MediaTypeNames.Application.Json, mapper);
        }
    }
}