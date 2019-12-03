using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Extensions;
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
        public const string ProfilePathRoot = "job-profiles";

        private readonly ILogger<ProfileController> logger;
        private readonly IJobProfileService jobProfileService;
        private readonly AutoMapper.IMapper mapper;

        public ProfileController(ILogger<ProfileController> logger, IJobProfileService jobProfileService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //AOP: These should be coded as an Aspect
            logger.LogInformation($"{nameof(Index)} has been called");

            var viewModel = new IndexViewModel();
            var jobProfileModels = await jobProfileService.GetAllAsync().ConfigureAwait(false);

            if (jobProfileModels is null)
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }
            else
            {
                viewModel.Documents = (from a in jobProfileModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logger.LogInformation($"{nameof(Index)} has succeeded");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            //AOP: These should be coded as an Aspect
            logger.LogInformation($"{nameof(Document)} has been called with: {article}");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel is null)
            {
                logger.LogWarning($"{nameof(Document)} has returned not found: {article}");
                return NotFound();
            }

            var viewModel = mapper.Map<DocumentViewModel>(jobProfileModel);
            viewModel.Breadcrumb = BuildBreadcrumb(jobProfileModel);
            logger.LogInformation($"{nameof(Document)} has succeeded for: {article}");
            return this.NegotiateContentResult(viewModel);
        }

        [HttpPost]
        [Route("profile")]
        public async Task<IActionResult> Create([FromBody] Data.Models.JobProfileModel jobProfileModel)
        {
            //AOP: These should be coded as an Aspect
            logger.LogInformation($"{nameof(Create)} has been called with {jobProfileModel?.JobProfileId} for {jobProfileModel?.CanonicalName} with seq number {jobProfileModel?.SequenceNumber}");

            if (jobProfileModel is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Create(jobProfileModel).ConfigureAwait(false);
            logger.LogInformation($"{nameof(Create)} has upserted content for: {jobProfileModel.CanonicalName} - Response - {response}");
            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("profile")]
        public async Task<IActionResult> Update([FromBody] Data.Models.JobProfileModel jobProfileModel)
        {
            //AOP: These should be coded as an Aspect
            logger.LogInformation($"{nameof(Create)} has been called with {jobProfileModel?.JobProfileId} for {jobProfileModel?.CanonicalName} with seq number {jobProfileModel?.SequenceNumber}");

            if (jobProfileModel is null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Update(jobProfileModel).ConfigureAwait(false);
            logger.LogInformation($"{nameof(Create)} has upserted content for: {jobProfileModel.CanonicalName} - Response - {response}");
            return new StatusCodeResult((int)response);
        }

        [HttpPatch]
        [Route("profile/{documentId}/metadata")]
        public async Task<IActionResult> Patch([FromBody]JobProfileMetadata jobProfileMetaDataPatchModel, Guid documentId)
        {
            logger.LogInformation($"{nameof(Patch)} has been called with {documentId} for {jobProfileMetaDataPatchModel?.CanonicalName} with seq number {jobProfileMetaDataPatchModel?.SequenceNumber}");

            if (jobProfileMetaDataPatchModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.Update(jobProfileMetaDataPatchModel).ConfigureAwait(false);
            logger.LogInformation($"{nameof(Patch)} has patched content for: {jobProfileMetaDataPatchModel.CanonicalName}. Response status - {response}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshJobProfileSegment refreshJobProfileSegmentModel)
        {
            logger.LogInformation($"{nameof(Refresh)} has been called with {refreshJobProfileSegmentModel?.JobProfileId} for {refreshJobProfileSegmentModel?.CanonicalName} with seq number {refreshJobProfileSegmentModel?.SequenceNumber}");

            if (refreshJobProfileSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);
            logger.LogInformation($"{nameof(Refresh)} has upserted content for: {refreshJobProfileSegmentModel.CanonicalName} - Response - {response}");
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
            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                const string xForwardedProto = "x-forwarded-prot";
                const string xOriginalHost = "x-original-host";

                Request.Headers.TryGetValue(xForwardedProto, out var xForwardedProtoValue);
                Request.Headers.TryGetValue(xOriginalHost, out var xOriginalHostValue);

                string jpBaseName;

                if (!string.IsNullOrWhiteSpace(xForwardedProtoValue) && !string.IsNullOrWhiteSpace(xOriginalHostValue))
                {
                    jpBaseName = $"{xForwardedProtoValue}://{xOriginalHostValue}";
                }
                else
                {
                    jpBaseName = Request.GetBaseAddress().ToString();
                }

                viewModel.CanonicalUrl = $"{jpBaseName.TrimEnd('/')}/{ProfilePathRoot}/{jobProfileModel.CanonicalName}";
            }

            logger.LogInformation($"{nameof(Head)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("profile/{article}/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string article)
        {
            logger.LogInformation($"{nameof(Breadcrumb)} has been called");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            var viewModel = BuildBreadcrumb(jobProfileModel);

            logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/contents")]
        public IActionResult Contents()
        {
            logger.LogInformation($"{nameof(Contents)} has been called");

            return Redirect("/explore-careers");
        }

        [HttpGet]
        [Route("profile/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logger.LogInformation($"{nameof(Body)} has been called");

            var viewModel = new BodyViewModel();
            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);
                logger.LogInformation($"{nameof(Body)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, jobProfileModel.Segments);
            }

            var alternateJobProfileModel = await jobProfileService.GetByAlternativeNameAsync(article).ConfigureAwait(false);
            if (alternateJobProfileModel != null)
            {
                var alternateUrl = $"{Request.GetBaseAddress()}/{ProfilePathRoot}/{alternateJobProfileModel.CanonicalName}";
                logger.LogWarning($"{nameof(Body)} has been redirected for: {article} to {alternateUrl}");

                return RedirectPermanentPreserveMethod(alternateUrl);
            }

            logger.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
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

        private static BreadcrumbViewModel BuildBreadcrumb(Data.Models.JobProfileModel jobProfileModel)
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

        #endregion Define helper methods
    }
}