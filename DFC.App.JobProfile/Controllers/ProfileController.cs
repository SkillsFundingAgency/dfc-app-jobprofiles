// TODO: outsanding routine to fix, document overview
// where is the overview called from?
// should the overview support post?
// should the routine and the view name match??
// should the body route work
//      profile/id/profile? or should it be profile/id/body?
//      see the other component parts /head /hero
// how does registration work? can the routes be fixed?
#pragma warning disable S125 // Sections of code should not be commented out
using DFC.App.JobProfile.ViewSupport.Adapters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
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
        [Route("profile/{occupationName}")]
        [Route("profile/{occupationName}/contents")]
        public Task<IActionResult> Document(string occupationName) =>
            _viewAdapter.GetDocumentViewFor(occupationName, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/{occupationName}/htmlhead")]
        public Task<IActionResult> Head(string occupationName) =>
            _viewAdapter.GetHeadViewFor(occupationName, $"{Request.PathBase.Value}/{ProfilePathRoot}", View);

        [HttpGet]
        [Route("profile/{occupationName}/hero")]
        public Task<IActionResult> Hero(string occupationName) =>
            _viewAdapter.GetHeroBannerViewFor(occupationName, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/contents")]
        public IActionResult Body() =>
            Redirect("/explore-careers");

        [HttpGet]
        [Route("profile/{occupationID}/profile")]
        public Task<IActionResult> Body(Guid occupationID) =>
            _viewAdapter.GetBodyViewFor(occupationID, View);

        [HttpGet]
        [Route("profile/hero")]
        [Route("profile/htmlhead")]
        [Route("/search-results")]
        public IActionResult NoContentResponses() =>
            NoContent();

        //[HttpPost, HttpGet]
        //[Route("profile/overview/{article}")]
        //public async Task<IActionResult> DocumentOverview(string article)
        //{
        //    logService.LogInformation($"{nameof(Document)} has been called with: {article}");
        //    var jobProfileModel = await jobProfileService.GetByNameAsync(article);
        //    if (jobProfileModel is null)
        //    {
        //        logService.LogWarning($"{nameof(Document)} has returned not found: {article}");
        //        return NotFound();
        //    }
        //    var viewModel = mapper.Map<HeroViewModel>(jobProfileModel);
        //    viewModel.JobProfileWebsiteUrl = this.overviewDetails.Link + viewModel.JobProfileWebsiteUrl;
        //    logService.LogInformation($"{nameof(Document)} has succeeded for: {article}");
        //    return View("~/Views/Profile/_OverviewDysac.cshtml", viewModel);
        //}
    }
}