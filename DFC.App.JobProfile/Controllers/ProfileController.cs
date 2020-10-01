using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profiles";

        private readonly ILogService logService;
        private readonly IJobProfileService jobProfileService;
        private readonly ISharedContentService sharedContentService;
        private readonly AutoMapper.IMapper mapper;
        private readonly FeedbackLinks feedbackLinks;
        private readonly string[] redirectionHostWhitelist = { "f0d341973d3c8650e00a0d24f10df50a159f28ca9cedeca318f2e9054a9982a0", "de2280453aa81cc7216b408c32a58f5326d32b42e3d46aee42abed2bd902e474" };

        public ProfileController(ILogService logService, IJobProfileService jobProfileService, ISharedContentService sharedContentService, AutoMapper.IMapper mapper, FeedbackLinks feedbackLinks, string[] redirectionHostWhitelist = null)
        {
            this.logService = logService;
            this.jobProfileService = jobProfileService;
            this.sharedContentService = sharedContentService;
            this.mapper = mapper;
            this.feedbackLinks = feedbackLinks;
            this.redirectionHostWhitelist = redirectionHostWhitelist ?? this.redirectionHostWhitelist;
            this.sharedContentService = sharedContentService;
        }

        [HttpGet]
        [Route("profile/")]
        [Route("profile/{index}")]
        public async Task<IActionResult> Index()
        {
            //AOP: These should be coded as an Aspect
            logService.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var jobProfileModels = await jobProfileService.GetAllAsync().ConfigureAwait(false);

            if (jobProfileModels is null)
            {
                logService.LogWarning($"{nameof(Index)} has returned with no results");
            }
            else
            {
                viewModel.Documents = (from a in jobProfileModels.OrderBy(o => o.JobProfileWebsiteUrl)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logService.LogInformation($"{nameof(Index)} has succeeded");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}")]
        [Route("profile/{article}/contents")]
        public async Task<IActionResult> Document(string article)
        {
            logService.LogInformation($"{nameof(Document)} has been called with: {article}");

            var contentList = new List<string>() { "speaktoanadvisor", "skillsassessment", "notwhatyourlookingfor" };

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel is null)
            {
                logService.LogWarning($"{nameof(Document)} has returned not found: {article}");
                return NotFound();
            }

            var sharedContent = await sharedContentService.GetByNamesAsync(contentList).ConfigureAwait(false);
            jobProfileModel.SharedContent = sharedContent;

            var viewModel = mapper.Map<DocumentViewModel>(jobProfileModel);
            viewModel.Breadcrumb = BuildBreadcrumb(jobProfileModel);
            logService.LogInformation($"{nameof(Document)} has succeeded for: {article}");
            return this.NegotiateContentResult(viewModel);
        }

        [HttpPost]
        [Route("profile")]
        public async Task<IActionResult> Create([FromBody] JobProfileModel jobProfileModel)
        {
            //AOP: These should be coded as an Aspect
            logService.LogInformation($"{nameof(Create)} has been called with {jobProfileModel?.JobProfileId} for {jobProfileModel?.CanonicalName} with seq number {jobProfileModel?.SequenceNumber}");

            if (jobProfileModel is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Create(jobProfileModel).ConfigureAwait(false);
            logService.LogInformation($"{nameof(Create)} has upserted content for: {jobProfileModel.CanonicalName} - Response - {response}");
            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("profile")]
        public async Task<IActionResult> Update([FromBody] JobProfileModel jobProfileModel)
        {
            //AOP: These should be coded as an Aspect
            logService.LogInformation($"{nameof(Create)} has been called with {jobProfileModel?.JobProfileId} for {jobProfileModel?.CanonicalName} with seq number {jobProfileModel?.SequenceNumber}");

            if (jobProfileModel is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Update(jobProfileModel).ConfigureAwait(false);
            logService.LogInformation($"{nameof(Create)} has upserted content for: {jobProfileModel.CanonicalName} - Response - {response}");
            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("profile/{documentId}/metadata")]
        public async Task<IActionResult> Patch([FromBody]JobProfileMetadata jobProfileMetaDataPatchModel, Guid documentId)
        {
            logService.LogInformation($"{nameof(Patch)} has been called with {documentId} for {jobProfileMetaDataPatchModel?.CanonicalName} with seq number {jobProfileMetaDataPatchModel?.SequenceNumber}");

            if (jobProfileMetaDataPatchModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Update(jobProfileMetaDataPatchModel).ConfigureAwait(false);
            logService.LogInformation($"{nameof(Patch)} has patched content for: {jobProfileMetaDataPatchModel.CanonicalName}. Response status - {response}");

            return new StatusCodeResult((int)response);
        }

        //[HttpPut]
        //[HttpPost]
        //[Route("refresh")]
        //public async Task<IActionResult> Refresh([FromBody]RefreshJobProfileSegment refreshJobProfileSegmentModel)
        //{
        //    logService.LogInformation($"{nameof(Refresh)} has been called with {refreshJobProfileSegmentModel?.JobProfileId} for {refreshJobProfileSegmentModel?.CanonicalName} with seq number {refreshJobProfileSegmentModel?.SequenceNumber}");

        //    if (refreshJobProfileSegmentModel == null)
        //    {
        //        return BadRequest();
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //var response = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);
        //    logService.LogInformation($"{nameof(Refresh)} has upserted content for: {refreshJobProfileSegmentModel.CanonicalName} - Response - {response}");
        //    return new StatusCodeResult((int)response);
        //}
        [HttpDelete]
        [Route("profile/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logService.LogInformation($"{nameof(Delete)} has been called");

            var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileModel == null)
            {
                logService.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await jobProfileService.DeleteAsync(documentId).ConfigureAwait(false);

            logService.LogInformation($"{nameof(Delete)} has deleted content for: {jobProfileModel.CanonicalName}");

            return Ok();
        }

        [HttpGet]
        [Route("profile/hero")]
        [Route("profile/htmlhead")]
        [Route("/search-results")]
        public IActionResult NoContentResponses()
        {
            return NoContent();
        }

        [HttpGet]
        [Route("profile/{article}/htmlhead")]
        public async Task<IActionResult> Head(string article)
        {
            logService.LogInformation($"{nameof(Head)} has been called");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

            var viewModel = BuildHeadViewModel(jobProfileModel);
            logService.LogInformation($"{nameof(Head)} has returned content for: {article}");
            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}/hero")]
        public async Task<IActionResult> Hero(string article)
        {
            logService.LogInformation($"{nameof(Hero)} has been called");

            var viewModel = new HeroViewModel();
            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                _ = decimal.TryParse(viewModel.SalaryStarter, out decimal starterParsed);
                _ = decimal.TryParse(viewModel.SalaryExperienced, out decimal experiencedParsed);
                var salaryStarterParsed = starterParsed;
                var salaryExperiencedParsed = experiencedParsed;

                viewModel.SalaryStarter = (salaryStarterParsed > 0) ? salaryStarterParsed.ToString("C0") : viewModel.SalaryStarter;
                viewModel.SalaryExperienced = (salaryExperiencedParsed > 0) ? salaryExperiencedParsed.ToString("C0") : viewModel.SalaryExperienced;

                logService.LogInformation($"{nameof(Hero)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel);
            }

            logService.LogWarning($"{nameof(Hero)} has not returned any content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("profile/contents")]
        public IActionResult Body()
        {
            logService.LogInformation($"{nameof(Body)} has been called");

            return Redirect("/explore-careers");
        }

        [HttpGet]
        public async Task<IActionResult> Body(string article)
        {
            logService.LogInformation($"{nameof(Body)} has been called");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel != null)
            {
                // var viewModel = mapper.Map<BodyViewModel>(jobProfileModel);
                // logService.LogInformation($"{nameof(Body)} has returned content for: {article}");
                // viewModel.SmartSurveyJP = this.feedbackLinks.SmartSurveyJP;
                var viewModel = mapper.Map<DocumentViewModel>(jobProfileModel);
                viewModel.Breadcrumb = BuildBreadcrumb(jobProfileModel);
                logService.LogInformation($"{nameof(Document)} has succeeded for: {article}");
                return this.NegotiateContentResult(viewModel);
            }

            var alternateJobProfileModel = await jobProfileService.GetByAlternativeNameAsync(article).ConfigureAwait(false);
            if (alternateJobProfileModel != null)
            {
                var host = Request.GetBaseAddress();
                if (!IsValidHost(host))
                {
                    logService.LogWarning($"Invalid host {host}.");
                    return BadRequest($"Invalid host {host}.");
                }
                else
                {
                    var alternateUrl = $"{host}{ProfilePathRoot}/{alternateJobProfileModel.CanonicalName}";
                    logService.LogWarning($"{nameof(Body)} has been redirected for: {article} to {alternateUrl}");

                    return RedirectPermanentPreserveMethod(alternateUrl);
                }
            }

            logService.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
        }

        [HttpGet]
        [Route("profile/{documentId}/profile")]
        public async Task<IActionResult> Profile(Guid documentId)
        {
            logService.LogInformation($"{nameof(Profile)} has been called");

            var viewModel = new BodyViewModel();
            var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                logService.LogInformation($"{nameof(Profile)} has returned a profile for: {documentId}");

                return this.NegotiateContentResult(viewModel, jobProfileModel);
            }

            logService.LogWarning($"{nameof(Profile)} has not returned a profile for: {documentId}");

            return NoContent();
        }

        #region Static helper methods

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
                }

                return builder.ToString();
            }
        }

        private static BreadcrumbViewModel BuildBreadcrumb(JobProfileModel jobProfileModel)
        {
            var viewModel = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>()
                {
                    new BreadcrumbPathViewModel()
                    {
                        Route = "/",
                        Title = "Home",
                    },
                    new BreadcrumbPathViewModel
                    {
                        Route = $"/{ProfilePathRoot}",
                        Title = "Job Profiles",
                    },
                },
            };

            if (jobProfileModel != null)
            {
                var breadcrumbPathViewModel = new BreadcrumbPathViewModel
                {
                    Route = $"/{ProfilePathRoot}/{jobProfileModel.CanonicalName}",
                    Title = jobProfileModel.BreadcrumbTitle,
                };

                viewModel.Paths.Add(breadcrumbPathViewModel);
            }

            viewModel.Paths.Last().AddHyperlink = false;

            return viewModel;
        }

        private HeadViewModel BuildHeadViewModel(JobProfileModel jobProfileModel)
        {
            var headModel = new HeadViewModel();
            if (jobProfileModel == null)
            {
                return headModel;
            }

            headModel = mapper.Map<HeadViewModel>(jobProfileModel);
            headModel.CanonicalUrl = $"{Request.GetBaseAddress()}{ProfilePathRoot}/{jobProfileModel.CanonicalName}";
            return headModel;
        }

        #endregion Static helper methods

        #region Helper methods

        private bool IsValidHost(Uri host)
        {
            return host.IsLoopback || host.Host.Split(".").Any(s => redirectionHostWhitelist.Contains(ComputeSha256Hash(s.ToLower(CultureInfo.InvariantCulture))));
        }

        #endregion Helper methods
    }
}