using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using DFC.App.JobProfile.Data.Models.Overview;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.CompositeInterfaceModels.FindACourseClient;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Razor.Templating.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ProfileService
{
    public class JobProfileService : IJobProfileService
    {
        private readonly ICosmosRepository<JobProfileModel> repository;
        private readonly ISegmentService segmentService;
        private readonly IMapper mapper;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly IConfiguration configuration;
        private readonly ICourseSearchApiService client;
        private string status;

        public JobProfileService(
            ICosmosRepository<JobProfileModel> repository,
            ISegmentService segmentService,
            IMapper mapper,
            ILogService logService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IRazorTemplateEngine razorTemplateEngine,
            IConfiguration configuration,
            ICourseSearchApiService client)
        {
            this.repository = repository;
            this.segmentService = segmentService;
            this.mapper = mapper;
            this.logService = logService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
            status = configuration.GetSection("contentMode:contentMode").Get<string>();
            this.client = client;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }

        public async Task<IList<HealthCheckItem>> SegmentsHealthCheckAsync()
        {
            return await segmentService.SegmentsHealthCheckAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobProfileModel>> GetAllAsync()
        {
            return await repository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<JobProfileModel> GetByIdAsync(Guid documentId)
        {
            return await repository.GetAsync(d => d.DocumentId == documentId).ConfigureAwait(false);
        }

        public async Task<JobProfileModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrEmpty(status))
            {
                status = "PUBLISHED";
            }

            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            var overview = new SegmentModel();
            var currentOpportunity = new SegmentModel();

            try
            {
                overview = await GetOverviewSegment(canonicalName, status);
                //var hotobecome = await GetHowToBecomeSegment(canonicalName);

                //Get Current Opportunity data
                currentOpportunity = await GetCurrentOpportunities(canonicalName, status);

                //WaitUntil.Completed

                //etc...
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            //Instead of returning the data object below, we'll reconstruct it with the data that is required from the various segment calls.
            //var data = await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
            var data = new JobProfileModel();

            //TODO - will need to update this section and adjust the unit tests accordingly once the other segments have been added in.  
            if (data != null)
            {
                data.Segments = new List<SegmentModel>();
                //data.Segments.RemoveAt(6);
                data.Segments.Add(overview);
                data.Segments.Add(currentOpportunity);
            }

            return data;
        }

        /// <summary>
        /// Get current opportunities data for individual job profile
        /// </summary>
        /// <param name="canonicalName">Jobprofile url.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>Current Opportunitie Segment model</returns>
        public async Task<SegmentModel> GetCurrentOpportunities(string canonicalName, string filter)
        {
            var currentOpportunities = new SegmentModel();
            var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel();
            currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentDataModel();
            currentOpportunitiesSegmentModel.Data.Courses = new Courses();
            currentOpportunitiesSegmentModel.CanonicalName = canonicalName;

            //Get job profile cousekeyword and lars code
            var jobprfile = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>(string.Concat(ApplicationKeys.JobProfileCurrentOpportunitiesGetByUrlPrefix, "/", canonicalName), filter);

            //get couses by course key words
            if (!string.IsNullOrEmpty(jobprfile.JobProileCurrentOpportunitiesGetbyUrl.First().Coursekeywords))
            {
                string coursekeywords = jobprfile.JobProileCurrentOpportunitiesGetbyUrl.First().Coursekeywords;
                var results = await GetCourses(coursekeywords);
                var courseSearchResults = results.Courses?.ToList();

                var opportunities = new List<Opportunity>();
                if (courseSearchResults != null)
                {
                    opportunities = MapCourses(courseSearchResults, opportunities);
                }

                currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords = coursekeywords;
                currentOpportunitiesSegmentModel.Data.Courses.Opportunities = opportunities;
            }

            //get apprenticeship by lars code.
            if (!string.IsNullOrEmpty(jobprfile.JobProileCurrentOpportunitiesGetbyUrl.First().SOCCode.ContentItems.First().ApprenticeshipStandards.ContentItems.First().LARScode))
            {
                //get apprenticeship vacancy data by lars code.
            }

            currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships();
            currentOpportunitiesSegmentModel.Data.JobTitle = canonicalName;

            var currentOpportunitiesObject = JsonConvert.SerializeObject(currentOpportunitiesSegmentModel.Data, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

            var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/CurrentOpportunities/BodyData.cshtml", currentOpportunitiesSegmentModel.Data).ConfigureAwait(false);

            currentOpportunities = new SegmentModel
            {
                Segment = JobProfileSegment.CurrentOpportunities,
                JsonV1 = currentOpportunitiesObject,
                RefreshStatus = RefreshStatus.Success,
                Markup = new HtmlString(html),
            };

            return currentOpportunities;
        }

        /// <summary>
        /// Get Overview Segment data from NuGet packages.
        /// </summary>
        /// <param name="canonicalName">Jobprofile url.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>Overview Segment model.</returns>
        public async Task<SegmentModel> GetOverviewSegment(string canonicalName, string filter)
        {
            SegmentModel overview = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(string.Concat(ApplicationKeys.JobProfilesOverview, "/", canonicalName), filter);

                if (response.JobProfileOverview != null && response.JobProfileOverview.Count > 0)
                {
                    var mappedOverview = mapper.Map<OverviewApiModel>(response);
                    mappedOverview.Breadcrumb = BuildBreadcrumb(canonicalName, string.Empty, mappedOverview.Title);

                    var overviewObject = JsonConvert.SerializeObject(
                        mappedOverview,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy(),
                            },
                        });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Overview/BodyData.cshtml", mappedOverview).ConfigureAwait(false);

                    overview = new SegmentModel
                    {
                        Segment = Data.JobProfileSegment.Overview,
                        JsonV1 = overviewObject,
                        RefreshStatus = Data.Enums.RefreshStatus.Success,
                        Markup = new HtmlString(html),
                    };
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
            }

            return overview;
        }

        /// <summary>
        /// Get Courses from FAC client API.
        /// </summary>
        /// <param name="courseKeywords">Couses key words, such as 'building services engineering'.</param>
        /// <returns>CourseResponse contains list Courses.</returns>
        public async Task<CoursesReponse> GetCourses(string courseKeywords)
        {
            string cachekey = ApplicationKeys.JobProfileCurrentOpportunitiesGetByUrlPrefix + "/" + courseKeywords;
            var redisdata = await sharedContentRedisInterface.GetCurrentOpportunitiesData<CoursesReponse>(cachekey);
            if (redisdata == null)
            {
                redisdata = new CoursesReponse();
                try
                {
                    var result = await client.GetCoursesAsync(courseKeywords, true).ConfigureAwait(false);

                    redisdata.Courses = result.ToList();

                    var save = await sharedContentRedisInterface.SetCurrentOpportunitiesData<CoursesReponse>(redisdata, cachekey, 48);
                    if (!save)
                        throw new InvalidOperationException("Redis save process failed.");

                }
                catch (Exception ex)
                {
                    logService.LogError(ex.ToString());
                }
            }
            return redisdata;
        }

        public async Task<JobProfileModel> GetByAlternativeNameAsync(string alternativeName)
        {
            if (string.IsNullOrWhiteSpace(alternativeName))
            {
                throw new ArgumentNullException(nameof(alternativeName));
            }

            return await repository.GetAsync(d => d.AlternativeNames.Contains(alternativeName.ToLowerInvariant())).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Create(JobProfileModel jobProfileModel)
        {

            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            jobProfileModel.MetaTags = jobProfileModel.MetaTags is null ? new MetaTags() : jobProfileModel.MetaTags;
            jobProfileModel.Segments = jobProfileModel.Segments is null ? new List<SegmentModel>() : jobProfileModel.Segments;

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord != null)
            {
                return HttpStatusCode.AlreadyReported;
            }

            return await repository.UpsertAsync(jobProfileModel).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(JobProfileMetadata jobProfileMetadata)
        {
            if (jobProfileMetadata is null)
            {
                throw new ArgumentNullException(nameof(jobProfileMetadata));
            }

            var existingRecord = await GetByIdAsync(jobProfileMetadata.JobProfileId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingRecord.SequenceNumber > jobProfileMetadata.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var mappedRecord = mapper.Map(jobProfileMetadata, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> Update(JobProfileModel jobProfileModel)
        {
            if (jobProfileModel == null)
            {
                throw new ArgumentNullException(nameof(jobProfileModel));
            }

            var existingRecord = await GetByIdAsync(jobProfileModel.DocumentId).ConfigureAwait(false);
            if (existingRecord is null)
            {
                return HttpStatusCode.NotFound;
            }

            if (existingRecord.SequenceNumber > jobProfileModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var mappedRecord = mapper.Map(jobProfileModel, existingRecord);
            return await repository.UpsertAsync(mappedRecord).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> RefreshSegmentsAsync(RefreshJobProfileSegment refreshJobProfileSegmentModel)
        {
            if (refreshJobProfileSegmentModel is null)
            {
                throw new ArgumentNullException(nameof(refreshJobProfileSegmentModel));
            }

            //Check existing document
            var existingJobProfile = await GetByIdAsync(refreshJobProfileSegmentModel.JobProfileId).ConfigureAwait(false);
            if (existingJobProfile is null)
            {
                return HttpStatusCode.NotFound;
            }

            var existingItem = existingJobProfile.Segments.SingleOrDefault(s => s.Segment == refreshJobProfileSegmentModel.Segment);
            if (existingItem?.RefreshSequence > refreshJobProfileSegmentModel.SequenceNumber)
            {
                return HttpStatusCode.AlreadyReported;
            }

            var offlineSegmentData = segmentService.GetOfflineSegment(refreshJobProfileSegmentModel.Segment);
            var segmentData = await segmentService.RefreshSegmentAsync(refreshJobProfileSegmentModel).ConfigureAwait(false);
            if (existingItem is null)
            {
                segmentData.Markup = !string.IsNullOrEmpty(segmentData.Markup?.Value) ? segmentData.Markup : offlineSegmentData.OfflineMarkup;
                segmentData.Json ??= offlineSegmentData.OfflineJson;
                existingJobProfile.Segments.Add(segmentData);
            }
            else
            {
                var index = existingJobProfile.Segments.IndexOf(existingItem);
                var fallbackMarkup = !string.IsNullOrEmpty(existingItem.Markup?.Value) ? existingItem.Markup : offlineSegmentData.OfflineMarkup;
                segmentData.Markup = !string.IsNullOrEmpty(segmentData.Markup?.Value) ? segmentData.Markup : fallbackMarkup;
                segmentData.Json ??= existingItem.Json ?? offlineSegmentData.OfflineJson;

                existingJobProfile.Segments[index] = segmentData;
            }

            var result = await repository.UpsertAsync(existingJobProfile).ConfigureAwait(false);
            return segmentData.RefreshStatus == Data.Enums.RefreshStatus.Success ? result : HttpStatusCode.FailedDependency;
        }

        public async Task<bool> DeleteAsync(Guid documentId)
        {
            var result = await repository.DeleteAsync(documentId).ConfigureAwait(false);

            return result == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Mapping courses data with Opportunity object.
        /// </summary>
        /// <param name="courseSearchResults">List of courses result data.</param>
        /// <param name="opportunities">List of Opportunity object.</param>
        /// <returns> List of Opportunity object. </returns>
        private List<Opportunity> MapCourses(List<FindACourseClient.Course> courseSearchResults, List<Opportunity> opportunities)
        {
            foreach (var course in courseSearchResults)
            {
                var opportunity = mapper.Map<Opportunity>(course);

                var courseIdGuid = new Guid(opportunity.CourseId);
                var tLevelIdGuid = new Guid(opportunity.TLevelId);
                var urlPath = $"/find-a-course/";
                var urlQueryString = courseIdGuid == Guid.Empty && tLevelIdGuid != Guid.Empty
                    ? $"tdetails?tlevelId={opportunity.TLevelId}&tlevelLocationId={opportunity.TLevelLocationId}"
                    : $"course-details?CourseId={opportunity.CourseId}&r={opportunity.RunId}";
                opportunity.Url = $"{urlPath}{urlQueryString}";
                opportunities.Add(opportunity);

                logService.LogInformation($"{nameof(MapCourses)} added details for {course.CourseId} to list");
            }

            return opportunities;
        }

        private static BreadcrumbViewModel BuildBreadcrumb(string canonicalName, string routePrefix, string title)
        {
            var viewModel = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>()
                {
                    new BreadcrumbPathViewModel()
                    {
                        Route = $"/explore-careers",
                        Title = "Home: Explore careers",
                    },
                },
            };

            var breadcrumbPathViewModel = new BreadcrumbPathViewModel
            {
                Route = $"/{routePrefix}/{canonicalName}",
                Title = title,
            };

            viewModel.Paths.Add(breadcrumbPathViewModel);
            viewModel.Paths.Last().AddHyperlink = false;

            return viewModel;
        }
    }
}