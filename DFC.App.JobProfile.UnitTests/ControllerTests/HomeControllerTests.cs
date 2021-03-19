using DFC.App.JobProfile.Controllers;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.JobProfile.UnitTests.ControllerTests.HomeControllerTests
{
    public class HomeControllerErrorTests
    {
        private ILogger<HomeController> _mockLogger;

        [Fact]
        public void HomeControllerErrorHtmlReturnsSuccess()
        {
            // arrange
            var controller = BuildHomeController();

            // act
            var result = controller.Error();

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        }

        protected HomeController BuildHomeController()
        {
            _mockLogger = A.Fake<ILogger<HomeController>>();

            return new HomeController(_mockLogger)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                },
            };
        }
    }
}
