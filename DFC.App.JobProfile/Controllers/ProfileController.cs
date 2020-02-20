using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Exceptions;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profiles";
        private readonly HtmlString emptySegmentMarkup = new HtmlString("<div class=\"govuk-width-container\"><H3>Service Unavailable</H3></div>");

        private readonly ILogService logService;
        private readonly IJobProfileService jobProfileService;
        private readonly AutoMapper.IMapper mapper;
        private readonly FeedbackLinks feedbackLinks;

        public ProfileController(ILogService logService, IJobProfileService jobProfileService, AutoMapper.IMapper mapper, FeedbackLinks feedbackLinks)
        {
            this.logService = logService;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
            this.feedbackLinks = feedbackLinks;
        }

        [HttpGet]
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
                viewModel.Documents = (from a in jobProfileModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();

                logService.LogInformation($"{nameof(Index)} has succeeded");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("profile/{article}")]
        public async Task<IActionResult> Document(string article)
        {
            //AOP: These should be coded as an Aspect
            logService.LogInformation($"{nameof(Document)} has been called with: {article}");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel is null)
            {
                logService.LogWarning($"{nameof(Document)} has returned not found: {article}");
                return NotFound();
            }

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

        [HttpPut]
        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody]RefreshJobProfileSegment refreshJobProfileSegmentModel)
        {
            logService.LogInformation($"{nameof(Refresh)} has been called with {refreshJobProfileSegmentModel?.JobProfileId} for {refreshJobProfileSegmentModel?.CanonicalName} with seq number {refreshJobProfileSegmentModel?.SequenceNumber}");

            if (refreshJobProfileSegmentModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await jobProfileService.RefreshSegmentsAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);
            logService.LogInformation($"{nameof(Refresh)} has upserted content for: {refreshJobProfileSegmentModel.CanonicalName} - Response - {response}");
            return new StatusCodeResult((int)response);
        }

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

            var viewModel = new HeadViewModel();
            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);

            if (jobProfileModel != null)
            {
                mapper.Map(jobProfileModel, viewModel);

                viewModel.CanonicalUrl = $"{Request.GetBaseAddress()}{ProfilePathRoot}/{jobProfileModel.CanonicalName}";
            }

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

                logService.LogInformation($"{nameof(Hero)} has returned content for: {article}");

                return this.NegotiateContentResult(viewModel, jobProfileModel.Segments);
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
        [Route("profile/{article}/contents")]
        public async Task<IActionResult> Body(string article)
        {
            logService.LogInformation($"{nameof(Body)} has been called");

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(jobProfileModel);
                logService.LogInformation($"{nameof(Body)} has returned content for: {article}");
                viewModel.SmartSurveyJP = this.feedbackLinks.SmartSurveyJP;

                return ValidateJobProfile(viewModel, jobProfileModel);
            }

            var alternateJobProfileModel = await jobProfileService.GetByAlternativeNameAsync(article).ConfigureAwait(false);
            if (alternateJobProfileModel != null)
            {
                var alternateUrl = $"{Request.GetBaseAddress()}{ProfilePathRoot}/{alternateJobProfileModel.CanonicalName}";
                logService.LogWarning($"{nameof(Body)} has been redirected for: {article} to {alternateUrl}");

                return RedirectPermanentPreserveMethod(alternateUrl);
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

        private IActionResult ValidateJobProfile(BodyViewModel bodyViewModel, JobProfileModel jobProfileModel)
        {
            var overviewExists = bodyViewModel.Segments.Any(s => s.Segment == JobProfileSegment.Overview);
            var howToBecomeExists = bodyViewModel.Segments.Any(s => s.Segment == JobProfileSegment.HowToBecome);
            var whatItTakesExists = bodyViewModel.Segments.Any(s => s.Segment == JobProfileSegment.WhatItTakes);

            if (!overviewExists || !howToBecomeExists || !whatItTakesExists)
            {
                throw new InvalidProfileException($"JobProfile with Id {jobProfileModel.DocumentId} is missing critical segment information");
            }

            return this.ValidateMarkup(bodyViewModel, jobProfileModel);
        }

        private IActionResult ValidateMarkup(BodyViewModel bodyViewModel, JobProfileModel jobProfileModel)
        {
            if (bodyViewModel.Segments != null)
            {
                foreach (var segmentModel in bodyViewModel.Segments)
                {
                    var markup = segmentModel.Markup.Value;

                    if (!string.IsNullOrWhiteSpace(markup))
                    {
                        continue;
                    }

                    switch (segmentModel.Segment)
                    {
                        case JobProfileSegment.Overview:
                        case JobProfileSegment.HowToBecome:
                        case JobProfileSegment.WhatItTakes:
                            throw new InvalidProfileException($"JobProfile with Id {jobProfileModel.DocumentId} is missing markup for segment {segmentModel.Segment.ToString()}");

                        case JobProfileSegment.RelatedCareers:
                        case JobProfileSegment.CurrentOpportunities:
                        case JobProfileSegment.WhatYouWillDo:
                        case JobProfileSegment.CareerPathsAndProgression:
                            {
                                segmentModel.Markup = emptySegmentMarkup;
                                break;
                            }
                    }
                }
            }

            return this.NegotiateContentResult(bodyViewModel, jobProfileModel.Segments);
        }

        #endregion Define helper methods
    }
}