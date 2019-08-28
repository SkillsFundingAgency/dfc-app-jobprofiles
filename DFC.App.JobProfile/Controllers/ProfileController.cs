using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class ProfileController : Controller
    {
        public const string ProfilePathRoot = "profile";

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
        [Route("profile")]
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

            return View(viewModel);
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

                return View(viewModel);
            }

            logger.LogWarning($"{nameof(Document)} has returned no content for: {article}");

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
                    new BreadcrumbPathViewModel
                    {
                        Route = $"/{ProfilePathRoot}/{jobProfileModel.CanonicalName}",
                        Title = jobProfileModel.BreadcrumbTitle,
                    },
                },
            };

            viewModel.Paths.Last().AddHyperlink = false;

            return viewModel;
        }

        #endregion
    }
}