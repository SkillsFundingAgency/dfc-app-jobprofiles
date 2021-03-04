// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
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
        [Route("profile/{article}")]
        [Route("profile/{article}/contents")]
        public Task<IActionResult> Document(string article) =>
            _viewAdapter.GetDocumentViewFor(article, ProfilePathRoot, View);

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

        [HttpGet]
        [Route("profile/hero")]
        [Route("profile/htmlhead")]
        [Route("/search-results")]
        public IActionResult NoContentResponses() =>
            NoContent();

        [HttpGet]
        [Route("profile/{article}/htmlhead")]
        public Task<IActionResult> Head(string article) =>
            _viewAdapter.GetHeadViewFor(article, $"{Request.PathBase.Value}/{ProfilePathRoot}", View);

        [HttpGet]
        [Route("profile/{article}/hero")]
        public Task<IActionResult> Hero(string article) =>
            _viewAdapter.GetHeroBannerViewFor(article, ProfilePathRoot, View);

        [HttpGet]
        [Route("profile/contents")]
        public IActionResult Body() =>
            Redirect("/explore-careers");

        [HttpGet]
        [Route("profile/{documentId}/profile")]
        public Task<IActionResult> Body(Guid documentId) =>
            _viewAdapter.GetBodyViewFor(documentId, View);
    }
}