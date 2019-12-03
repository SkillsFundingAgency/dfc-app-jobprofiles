using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Extensions;
using DFC.App.JobProfile.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ILogger<SitemapController> logger;
        private readonly IJobProfileService jobProfileService;

        public SitemapController(ILogger<SitemapController> logger, IJobProfileService jobProfileService)
        {
            this.logger = logger;
            this.jobProfileService = jobProfileService;
        }

        [HttpGet]
        public async Task<ContentResult> Sitemap()
        {
            try
            {
                logger.LogInformation("Generating Sitemap");

                var jpBaseName = Request.GetBaseAddress()?.ToString().TrimEnd('/');
                var sitemapUrlPrefix = $"{jpBaseName}/{ProfileController.ProfilePathRoot}";
                var sitemap = new Sitemap();

                var jobProfileModels = await jobProfileService.GetAllAsync().ConfigureAwait(false);

                if (jobProfileModels != null)
                {
                    var jobProfileModelsList = jobProfileModels.ToList();

                    if (jobProfileModelsList.Any())
                    {
                        var sitemapJobProfileModels = jobProfileModelsList
                             .Where(w => w.IncludeInSitemap)
                             .OrderBy(o => o.CanonicalName);

                        foreach (var jobProfileModel in sitemapJobProfileModels)
                        {
                            sitemap.Add(new SitemapLocation
                            {
                                Url = $"{sitemapUrlPrefix}/{jobProfileModel.CanonicalName}",
                                Priority = 1,
                            });
                        }
                    }
                }

                // extract the sitemap
                var xmlString = sitemap.WriteSitemapToString();

                logger.LogInformation("Generated Sitemap");

                return Content(xmlString, MediaTypeNames.Application.Xml);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{nameof(Sitemap)}: {ex.Message}");
            }

            return null;
        }
    }
}