// TODO: fix(?) me!
#pragma warning disable S125 // Sections of code should not be commented out
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using DFC.App.JobProfile.ViewSupport.Adapters;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profiles";

        private readonly IAdaptProfileDocumentViews _viewAdapter;

        public ProfileController(
            IAdaptProfileDocumentViews viewAdapter)
        {
            _viewAdapter = viewAdapter;
        }

        [HttpGet]
        [Route("profile/")]
        public async Task<IActionResult> Index() =>
            await _viewAdapter.GetSummaryDocuments<IndexViewModel>(View);

        [HttpGet]
        [Route("profile/{article}")]
        [Route("profile/{article}/contents")]
        public async Task<IActionResult> Document(string article) =>
            await _viewAdapter.GetDocumentViewFor<DocumentViewModel>(article, View);

        //[HttpPost, HttpGet]
        //[Route("profile/overview/{article}")]
        //public async Task<IActionResult> DocumentOverview(string article)
        //{
        //    logService.LogInformation($"{nameof(Document)} has been called with: {article}");

        //    var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
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

        //[HttpGet]
        //[Route("profile/hero")]
        //[Route("profile/htmlhead")]
        //[Route("/search-results")]
        //public IActionResult NoContentResponses()
        //{
        //    return NoContent();
        //}

        //[HttpGet]
        //[Route("profile/{article}/htmlhead")]
        //public async Task<IActionResult> Head(string article)
        //{
        //    logService.LogInformation($"{nameof(Head)} has been called");

        //    var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

        //    var viewModel = BuildHeadViewModel(jobProfileModel);
        //    logService.LogInformation($"{nameof(Head)} has returned content for: {article}");
        //    return this.NegotiateContentResult(viewModel);
        //}

        //[HttpGet]
        //[Route("profile/{article}/hero")]
        //public async Task<IActionResult> Hero(string article)
        //{
        //    logService.LogInformation($"{nameof(Hero)} has been called");

        //    var viewModel = new HeroViewModel();
        //    var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

        //    if (jobProfileModel != null)
        //    {
        //        mapper.Map(jobProfileModel, viewModel);

        //        logService.LogInformation($"{nameof(Hero)} has returned content for: {article}");

        //        return this.NegotiateContentResult(viewModel);
        //    }

        //    logService.LogWarning($"{nameof(Hero)} has not returned any content for: {article}");

        //    return NoContent();
        //}

        //[HttpGet]
        //[Route("profile/contents")]
        //public IActionResult Body()
        //{
        //    logService.LogInformation($"{nameof(Body)} has been called");

        //    return Redirect("/explore-careers");
        //}

        //[HttpGet]
        //[Route("profile/{documentId}/profile")]
        //public async Task<IActionResult> Profile(Guid documentId)
        //{
        //    logService.LogInformation($"{nameof(Profile)} has been called");

        //    var viewModel = new BodyViewModel();
        //    var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

        //    if (jobProfileModel != null)
        //    {
        //        mapper.Map(jobProfileModel, viewModel);

        //        logService.LogInformation($"{nameof(Profile)} has returned a profile for: {documentId}");

        //        return this.NegotiateContentResult(viewModel, jobProfileModel);
        //    }

        //    logService.LogWarning($"{nameof(Profile)} has not returned a profile for: {documentId}");

        //    return NoContent();
        //}

        //private static BreadcrumbViewModel BuildBreadcrumb(JobProfileModel jobProfileModel)
        //{
        //    var viewModel = new BreadcrumbViewModel
        //    {
        //        Paths = new List<BreadcrumbPathViewModel>()
        //        {
        //            new BreadcrumbPathViewModel()
        //            {
        //                Route = "/",
        //                Title = "Home",
        //            },
        //            new BreadcrumbPathViewModel
        //            {
        //                Route = $"/{ProfilePathRoot}",
        //                Title = "Job Profiles",
        //            },
        //        },
        //    };

        //    if (jobProfileModel != null)
        //    {
        //        var breadcrumbPathViewModel = new BreadcrumbPathViewModel
        //        {
        //            Route = $"/{ProfilePathRoot}/{jobProfileModel.CanonicalName}",
        //            Title = jobProfileModel.BreadcrumbTitle,
        //        };

        //        viewModel.Paths.Add(breadcrumbPathViewModel);
        //    }

        //    viewModel.Paths.Last().AddHyperlink = false;

        //    return viewModel;
        //}

        //private HeadViewModel BuildHeadViewModel(JobProfileModel jobProfileModel)
        //{
        //    var headModel = new HeadViewModel();
        //    if (jobProfileModel == null)
        //    {
        //        return headModel;
        //    }

        //    headModel = mapper.Map<HeadViewModel>(jobProfileModel);
        //    headModel.CanonicalUrl = $"{Request.GetBaseAddress()}{ProfilePathRoot}/{jobProfileModel.CanonicalName}";
        //    return headModel;
        //}
    }
}