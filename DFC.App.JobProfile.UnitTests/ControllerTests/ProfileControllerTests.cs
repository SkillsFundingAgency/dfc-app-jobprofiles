using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.ViewSupport.Adapters;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests
{
    public sealed class ProfileControllerTests
    {
        private const string AnyOldArticleName = "any-old-article-name";

        [Fact]
        public async Task JobProfileControllerdocumentReturnsCorrectType()
        {
            // arrange
            var controller = BuildProfileController();

            // act
            var result = await controller.Document(AnyOldArticleName);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);
        }

        public ProfileController BuildProfileController() =>
            new ProfileController(A.Fake<IAdaptProfileDocumentViews>());
    }
}