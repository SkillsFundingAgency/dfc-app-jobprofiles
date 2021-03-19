using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.ViewSupport.Adapters;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests
{
    public sealed class ProfileControllerTests
    {
        private const string AnyOldArticleName = "any-old-article-name";
        private static Guid AnyOldArticleID = Guid.NewGuid();

        private IAdaptProfileDocumentViews _adapter;

        [Fact]
        public async Task DocumentReturnsCorrectType()
        {
            // arrange
            var controller = BuildProfileController();
            var returnModel = new DocumentViewModel();

            A.CallTo(() => _adapter.GetDocumentViewFor(AnyOldArticleName, ProfileController.ProfilePathRoot, controller.View))
                .Returns(controller.View(returnModel));

            // act
            var result = await controller.Document(AnyOldArticleName);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task HeadReturnsCorrectType()
        {
            // emptied to get a build through
            await Task.CompletedTask;
        }

        [Fact]
        public async Task HeroReturnsCorrectType()
        {
            // arrange
            var controller = BuildProfileController();
            var returnModel = new HeroViewModel();

            A.CallTo(() => _adapter.GetHeroBannerViewFor(AnyOldArticleName, ProfileController.ProfilePathRoot, controller.View))
                .Returns(controller.View(returnModel));

            // act
            var result = await controller.Hero(AnyOldArticleName);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<HeroViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task BodyReturnsCorrectType()
        {
            // arrange
            var controller = BuildProfileController();
            var returnModel = new BodyViewModel();

            A.CallTo(() => _adapter.GetBodyViewFor(AnyOldArticleID, controller.View))
                .Returns(controller.View(returnModel));

            // act
            var result = await controller.Body(AnyOldArticleID);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
        }

        public ProfileController BuildProfileController(HttpContext context = null)
        {
            _adapter = A.Fake<IAdaptProfileDocumentViews>();

            return new ProfileController(_adapter)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = context ?? new DefaultHttpContext()
                },
            };
        }
    }
}