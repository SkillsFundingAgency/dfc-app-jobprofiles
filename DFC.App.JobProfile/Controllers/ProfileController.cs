using DFC.App.JobProfile.ViewSupport.Adapters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profiles";

        private readonly IAdaptProfileDocumentViews _viewAdapter;

        public ProfileController(IAdaptProfileDocumentViews viewAdapter) =>
            _viewAdapter = viewAdapter;

        [HttpGet]
        [Route("profile/")]
        public Task<IActionResult> Index() =>
            _viewAdapter.GetSummaryView(View);

        [HttpGet]
        [Route("profile/{occupationName}/htmlhead")]
        public Task<IActionResult> Head(string occupationName) =>
            _viewAdapter.GetHeadViewFor(occupationName, $"{Request.PathBase.Value}/{ProfilePathRoot}", View);

        [HttpGet]
        [Route("profile/{occupationName}/hero")]
        public Task<IActionResult> Hero(string occupationName) =>
            _viewAdapter.GetHeroBannerViewFor(occupationName, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/{occupationID}/body")]
        public Task<IActionResult> Body(Guid occupationID) =>
            _viewAdapter.GetBodyViewFor(occupationID, View);

        [HttpGet]
        [Route("profile/{occupationName}/overview")]
        public Task<IActionResult> DocumentOverview(string occupationName) =>
            _viewAdapter.GetDocumentOverviewFor(occupationName, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/{occupationName}")]
        [Route("profile/{occupationName}/contents")]
        public Task<IActionResult> Document(string occupationName) =>
            _viewAdapter.GetDocumentViewFor(occupationName, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/contents")]
        public IActionResult Contents() =>
            Redirect("/explore-careers");

        [HttpGet]
        [Route("profile/hero")]
        [Route("profile/htmlhead")]
        [Route("/search-results")]
        public IActionResult NoContentResponses() =>
            NoContent();
    }
}