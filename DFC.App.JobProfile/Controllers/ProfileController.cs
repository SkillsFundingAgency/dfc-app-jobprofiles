﻿using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using DFC.App.JobProfile.Exceptions;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.Models;
using DFC.App.JobProfile.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "job-profiles";

        private const string ExpiryAppSettings = "Cms:Expiry";
        private readonly IConfiguration configuration;
        private readonly ILogService logService;
        private readonly IJobProfileService jobProfileService;
        private readonly AutoMapper.IMapper mapper;
        private readonly ConfigValues configValues;
        private readonly FeedbackLinks feedbackLinks;
        private readonly IRedirectionSecurityService redirectionSecurityService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly string status;
        private double expiryInHours = 4;

        public ProfileController(ILogService logService, IJobProfileService jobProfileService, AutoMapper.IMapper mapper, ConfigValues configValues, FeedbackLinks feedbackLinks, IRedirectionSecurityService redirectionSecurityService, ISharedContentRedisInterface sharedContentRedisInterface, IConfiguration configuration)
        {
            this.logService = logService;
            this.jobProfileService = jobProfileService;
            this.mapper = mapper;
            this.configuration = configuration;
            this.configValues = configValues;
            this.feedbackLinks = feedbackLinks;
            this.redirectionSecurityService = redirectionSecurityService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            status = configuration.GetSection("contentMode:contentMode").Get<string>();
            if (this.configuration != null)
            {
                string expiryAppString = this.configuration.GetSection(ExpiryAppSettings).Get<string>();
                if (double.TryParse(expiryAppString, out var expiryAppStringParseResult))
                {
                    expiryInHours = expiryAppStringParseResult;
                }
            }
        }

        [HttpGet]
        [Route("profile/index")]
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

        [HttpGet]
        [Route("profile/hero")]
        [Route("profile/htmlhead")]
        [Route("/search-results")]
        public IActionResult NoContentResponses()
        {
            logService.LogInformation($"{nameof(NoContentResponses)} has been called");
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
                viewModel.ShowLmi = configValues.EnableLMI;

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
            var host = Request.GetBaseAddress();
            if (!redirectionSecurityService.IsValidHost(host))
            {
                logService.LogWarning($"Invalid host {host}.");
                return BadRequest($"Invalid host {host}.");
            }

            var jobProfileModel = await jobProfileService.GetByNameAsync(article).ConfigureAwait(false);
            if (jobProfileModel != null)
            {
                var viewModel = mapper.Map<BodyViewModel>(jobProfileModel);
                var speakToAnAdviser = await sharedContentRedisInterface.GetDataAsyncWithExpiry<SharedHtml>(ApplicationKeys.SpeakToAnAdviserSharedContent, status, expiryInHours);
                viewModel.SpeakToAnAdviser = new StaticContentItemModel()
                {
                    Content = speakToAnAdviser.Html,
                };

                logService.LogInformation($"{nameof(Body)} has returned content for: {article}");
                viewModel.SmartSurveyJP = feedbackLinks.SmartSurveyJP;

                return ValidateJobProfile(viewModel, jobProfileModel);
            }

            logService.LogWarning($"{nameof(Body)} has not returned any content for: {article}");
            return NotFound();
        }

        [HttpPost]
        [Route("refreshCourses")]
        public async Task<IActionResult> RefreshCourses([FromBody] JobProfileCurrentOpportunitiesSearchModel jobProfileModel)
        {
            logService.LogInformation($"{nameof(RefreshCourses)} has been called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (jobProfileModel != null)
            {
                var response = await jobProfileService.RefreshCourses("PUBLISHED", jobProfileModel.First, jobProfileModel.Skip).ConfigureAwait(false);
                logService.LogInformation($"{nameof(RefreshCourses)} has upserted content for: " + response.ToString());
            }
            else
            {
                return NoContent();
            }

            return Ok();
        }

        [HttpPost]
        [Route("refreshApprenticeships")]
        public async Task<IActionResult> RefreshApprenticeships([FromBody] JobProfileCurrentOpportunitiesSearchModel jobProfileModel)
        {
            logService.LogInformation($"{nameof(RefreshApprenticeships)} has been called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (jobProfileModel != null)
            {
                var response = await jobProfileService.RefreshApprenticeshipsAsync("PUBLISHED", jobProfileModel.First, jobProfileModel.Skip).ConfigureAwait(false);
                logService.LogInformation($"{nameof(RefreshApprenticeships)} has upserted content for: " + response.ToString());
            }
            else
            {
                return NoContent();
            }

            return Ok();
        }

        [HttpPost]
        [Route("refreshAllSegments")]
        public async Task<IActionResult> RefreshAllSegments([FromBody] JobProfileCurrentOpportunitiesSearchModel jobProfileModel)
        {
            logService.LogInformation($"{nameof(RefreshAllSegments)} has been called");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (jobProfileModel != null)
            {
                var response = await jobProfileService.RefreshAllSegments("PUBLISHED", jobProfileModel.First, jobProfileModel.Skip).ConfigureAwait(false);

                logService.LogInformation($"{nameof(RefreshAllSegments)} has upserted content for: " + response.ToString());
            }
            else
            {
                return NoContent();
            }

            return Ok();
        }

        [HttpGet]
        [Route("countJobProfiles")]
        public async Task<IActionResult> CountJobProfiles()
        {
            logService.LogInformation($"{nameof(CountJobProfiles)} has been called");

            var response = await jobProfileService.GetAllAsync().ConfigureAwait(false);
            var count = response.Count();
            logService.LogInformation($"{nameof(CountJobProfiles)} has found " + count + " job profiles.");
            return Ok(count);
        }

        #region Static helper methods

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
                throw new InvalidProfileException($"JobProfile {jobProfileModel.CanonicalName} is missing critical segment information");
            }

            return ValidateMarkup(bodyViewModel, jobProfileModel);
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
                            throw new InvalidProfileException($"JobProfile {jobProfileModel.CanonicalName} is missing markup for segment {segmentModel.Segment}");
                    }
                }
            }

            return this.NegotiateContentResult(bodyViewModel, jobProfileModel.Segments);
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
    }
}