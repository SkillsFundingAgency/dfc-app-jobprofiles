using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.PatchModels;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profile";

        private readonly ILogger<ProfileController> logger;
        private readonly IJobProfileService jobProfileService;
        private readonly AutoMapper.IMapper mapper;
        private readonly BrandingAssetsModel brandingAssetsModel;

        public ProfileController(ILogger<ProfileController> logger, IJobProfileService jobProfileService, AutoMapper.IMapper mapper, BrandingAssetsModel brandingAssetsModel)
        {
            this.logger = logger;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
            this.brandingAssetsModel = brandingAssetsModel;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var jobProfileModels = await jobProfileService.GetAllAsync().ConfigureAwait(false);

            if (jobProfileModels != null)
            {
                viewModel.Documents = (from a in jobProfileModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            logger.LogInformation($"{nameof(Document)} has been called with: {article}");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                var viewModel = mapper.Map<DocumentViewModel>(jobProfileModel);

                viewModel.Breadcrumb = BuildBreadcrumb(jobProfileModel);

                logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");

                return this.NegotiateContentResult(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

            return NoContent();
        }

        [HttpPut]
        [HttpPost]
        [Route("profile")]
        public async Task<IActionResult> CreateOrUpdate([FromBody]JobProfileModel jobProfileModel)
        {
            logger.LogInformation($"{nameof(CreateOrUpdate)} has been called");

            if (jobProfileModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.UpsertAsync(jobProfileModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(CreateOrUpdate)} has upserted content for: {jobProfileModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [HttpPost]
        [Route("profile/refresh")]
        public async Task<IActionResult> PostRefresh([FromBody]RefreshJobProfileSegmentModel refreshJobProfileSegmentModel)
        {
            logger.LogInformation($"{nameof(PostRefresh)} has been called");

            if (refreshJobProfileSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestBaseAddress = Request.RequestBaseAddress(Url);

            var existingJobProfileModel = await jobProfileService.GetByIdAsync(refreshJobProfileSegmentModel.DocumentId).ConfigureAwait(false);

            if (existingJobProfileModel != null)
            {
                var response = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel, existingJobProfileModel, requestBaseAddress).ConfigureAwait(false);

                logger.LogInformation($"{nameof(PostRefresh)} has upserted content for: {existingJobProfileModel.CanonicalName}");

                return new StatusCodeResult((int)response);
            }

            return NoContent();
        }

        [HttpPatch]
        [Route("profile/{documentId}/metadata")]
        public async Task<IActionResult> Patch([FromBody]JobProfileMetaDataPatchModel jobProfileMetaDataPatchModel, Guid documentId)
        {
            logger.LogInformation($"{nameof(Patch)} has been called");

            if (jobProfileMetaDataPatchModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NoContent();
            }

            mapper.Map(jobProfileMetaDataPatchModel, jobProfileModel);

            var response = await jobProfileService.UpsertAsync(jobProfileModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Patch)} has patched content for: {jobProfileModel.CanonicalName}");

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("profile/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            logger.LogInformation($"{nameof(Delete)} has been called");

            var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileModel == null)
            {
                logger.LogWarning($"{nameof(Document)} has returned no content for: {documentId}");

                return NotFound();
            }

            await jobProfileService.DeleteAsync(documentId).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Delete)} has deleted content for: {jobProfileModel.CanonicalName}");

            return Ok();
        }

        [HttpGet]
        [Route("profile/{article}/htmlhead")]
        public async Task<IActionResult> Head(string article)
        {
            logger.LogInformation($"{nameof(Head)} has been called");

            var viewModel = new HeadViewModel();
            var jobProfileModel = await jobProfileService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                viewModel.CanonicalUrl = $"{Request.Scheme}://{Request.Host}/{ProfilePathRoot}/{jobProfileModel.CanonicalName}";
            }

            viewModel.CssLink = brandingAssetsModel.AppCssFilePath;

            logger.LogInformation($"{nameof(Head)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("profile/{article}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string article)
        {
            logger.LogInformation($"{nameof(Breadcrumb)} has been called");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);
            var viewModel = BuildBreadcrumb(jobProfileModel);

            logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{nameof(Body)} has been called");

            var viewModel = new BodyViewModel();
            var jobProfileModel = await jobProfileService.GetByNameAsync(article, Request.IsDraftRequest()).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                logger.LogInformation($"{nameof(Body)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, jobProfileModel.Data);
            }

            var alternateJobProfileModel = await jobProfileService.GetByAlternativeNameAsync(article).ConfigureAwait(false);

            if (alternateJobProfileModel != null)
            {
                var alternateUrl = $"{Request.Scheme}://{Request.Host}/{ProfilePathRoot}/{alternateJobProfileModel.CanonicalName}";

                logger.LogWarning($"{nameof(Body)} has been redirected for: {article} to {alternateUrl}");

                return RedirectPermanentPreserveMethod(alternateUrl);
            }

            logger.LogWarning($"{nameof(Body)} has not returned any content for: {article}");

            return NoContent();
        }

        [HttpGet]
        [Route("profile/{documentId}/profile")]
        public async Task<IActionResult> Profile(Guid documentId)
        {
            logger.LogInformation($"{nameof(Profile)} has been called");

            var viewModel = new BodyViewModel();
            var jobProfileModel = await jobProfileService.GetByIdAsync(documentId).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                logger.LogInformation($"{nameof(Profile)} has returned a profile for: {documentId}");

                return this.NegotiateContentResult(viewModel, jobProfileModel);
            }

            logger.LogWarning($"{nameof(Profile)} has not returned a profile for: {documentId}");

            return NoContent();
        }

        #region Define helper methods

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

        #endregion
    }
}