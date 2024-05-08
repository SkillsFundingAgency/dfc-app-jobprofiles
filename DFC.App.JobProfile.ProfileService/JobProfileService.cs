using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CareerPath;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using DFC.App.JobProfile.Data.Models.Overview;
using DFC.App.JobProfile.Data.Models.RelatedCareersModels;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Data.Models.SkillsModels;
using DFC.App.JobProfile.ProfileService.Models;
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
using Microsoft.Extensions.Hosting.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Razor.Templating.Core;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Skills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Skills;
using JobProfSkills = DFC.App.JobProfile.Data.Models.SkillsModels.Skills;
using NHibernate.Criterion;

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
        private string status = string.Empty;

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
            this.client = client;
            status = configuration.GetSection("contentMode:contentMode").Get<string>() ?? "PUBLISHED";
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

            var howToBecome = new SegmentModel();
            var relatedCareers = new SegmentModel();
            var overview = new SegmentModel();
            var careersPath = new SegmentModel();
            var skills = new SegmentModel();
            var currentOpportunity = new SegmentModel();

            try
            {
                //howToBecome = await GetHowToBecomeSegmentAsync(canonicalName, status);
                overview = await GetOverviewSegment(canonicalName, status);
                relatedCareers = await GetRelatedCareersSegmentAsync(canonicalName, status);
                careersPath = await GetCareerPathSegmentAsync(canonicalName, status);
                skills = await GetSkillSegmentAsync(canonicalName, status);

                //Get Current Opportunity data

                currentOpportunity = await GetCurrentOpportunities(canonicalName, status);

                //WaitUntil.Completed

                //var data = await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);

                //For developer, when debugging there is no data from Cosmos DB, we need initial data value. This can be deleted when deploying
                var data = await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);

                /* if (data != null && overview.Markup != null)
                 {
                     data.Segments = new List<SegmentModel>();
                     data.Segments.Add(howToBecome);
                     data.Segments.Add(relatedCareers);
                     data.Segments.Add(overview);
                     data.Segments.Add(currentOpportunity);
                     data.Segments.Add(skills);
                     data.Segments.Add(careersPath);
                 }*/

                if (data != null && howToBecome != null && overview != null && relatedCareers != null && careersPath != null)
                {
                   /* int index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.HowToBecome));
                    data.Segments[index] = howToBecome;*/
                    int index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.RelatedCareers));
                    data.Segments[index] = relatedCareers;
                    index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.Overview));
                    data.Segments[index] = overview;
                    index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.CareerPathsAndProgression));
                    data.Segments[index] = careersPath;
                    index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.WhatItTakes));
                    data.Segments[index] = skills;
                    index = data.Segments.IndexOf(data.Segments.FirstOrDefault(s => s.Segment == JobProfileSegment.CurrentOpportunities));
                    data.Segments[index] = currentOpportunity;
                }

                return data;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }
        }

        public async Task<SegmentModel> GetRelatedCareersSegmentAsync(string canonicalName, string status)
        {
            var relatedCareers = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(ApplicationKeys.JobProfileRelatedCareersPrefix + "/" + canonicalName, status);

                if (response.JobProfileRelatedCareers != null)
                {
                    var mappedResponse = mapper.Map<RelatedCareerSegmentDataModel>(response);

                    var relatedCareersObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/RelatedCareers/RelatedCareersBody.cshtml", mappedResponse).ConfigureAwait(false);

                    relatedCareers = new SegmentModel
                    {
                        Segment = JobProfileSegment.RelatedCareers,
                        JsonV1 = relatedCareersObject,
                        RefreshStatus = RefreshStatus.Success,
                        Markup = new HtmlString(html),
                    };
                }

                return relatedCareers;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }
        }

        public async Task<SegmentModel> GetHowToBecomeSegmentAsync(string canonicalName, string filter)
        {
            var howToBecome = new SegmentModel();

            try
            {
                // Get the response from GraphQl
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(ApplicationKeys.JobProfileHowToBecome + "/" + canonicalName, filter);

                // Map the response to a HowToBecomeSegmentDataModel
                var mappedResponse = mapper.Map<HowToBecomeSegmentDataModel>(response);

                // Map CommonRoutes for College
                var collegeCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.College);

                // Map CommonRoutes for University
                var universityCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.University);

                // Map CommonRoutes for Apprenticeship
                var apprenticeshipCommonRoutes = mapper.Map<CommonRoutes>(response, opt => opt.Items["RouteName"] = RouteName.Apprenticeship);

                // Combine CommonRoutes into a list
                var allCommonRoutes = new List<CommonRoutes>
                {
                    collegeCommonRoutes,
                    universityCommonRoutes,
                    apprenticeshipCommonRoutes,
                };

                // Combine the CommonRoutes with the mapped response
                if (mappedResponse.EntryRoutes != null)
                {
                    mappedResponse.EntryRoutes.CommonRoutes = allCommonRoutes;
                }

                // Serialize the mapped response into an object
                var howToBecomeObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },
                });

                // Render the CSHTML to string
                var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/HowToBecome/Body.cshtml", mappedResponse).ConfigureAwait(false);

                // Build the SegmentModel
                howToBecome = new SegmentModel
                {
                    Segment = Data.JobProfileSegment.HowToBecome,
                    Markup = new HtmlString(html),
                    JsonV1 = howToBecomeObject,
                    RefreshStatus = RefreshStatus.Success,
                };

                return howToBecome;
            }
            catch (Exception e)
            {
                logService.LogError(e.ToString());
                throw;
            }
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
            if (!string.IsNullOrEmpty(jobprfile.JobProileCurrentOpportunitiesGetbyUrl[0].Coursekeywords))
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
            if (jobprfile.JobProileCurrentOpportunitiesGetbyUrl[0].SOCCode?.ContentItems.Count() > 0 && jobprfile.JobProileCurrentOpportunitiesGetbyUrl[0].SOCCode?.ContentItems?[0].ApprenticeshipStandards.ContentItems.Count() > 0)
            {
                if (!string.IsNullOrEmpty(jobprfile.JobProileCurrentOpportunitiesGetbyUrl[0].SOCCode?.ContentItems?[0].ApprenticeshipStandards.ContentItems?[0].LARScode))
                {
                    //get apprenticeship vacancy data by lars code.
                }
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

        public async Task<SegmentModel> GetCareerPathSegmentAsync(string canonicalName, string status)
        {
            SegmentModel careerPath = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(ApplicationKeys.JobProfilesCarreerPath + "/" + canonicalName, status);

                if (response.JobProileCareerPath != null)
                {
                    var mappedResponse = mapper.Map<CareerPathSegmentDataModel>(response);

                    var careerPathObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/CareerPath/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

                    careerPath = new SegmentModel
                    {
                        Segment = JobProfileSegment.CareerPathsAndProgression,
                        JsonV1 = careerPathObject,
                        RefreshStatus = RefreshStatus.Success,
                        Markup = new HtmlString(html),
                    };
                }

                return careerPath;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }
        }

        /// <summary>
        /// Method to retrieve the segment data for the "What it Takes" section on a job-profiles page.
        /// </summary>
        /// <param name="canonicalName"> Contains the name of the job profile.</param>
        /// <param name="status"> Contains the contentMode variable value used to determine whether to retrieve data from draft or published on graphQL.</param>
        /// <returns>Returns segment information containing HTML markup data to render the "What it Takes" segment.</returns>
        public async Task<SegmentModel> GetSkillSegmentAsync(string canonicalName, string status)
        {
            SegmentModel skills = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(ApplicationKeys.JobProfileSkillsSuffix + "/" + canonicalName, status);
                var skillsResponse = await sharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(ApplicationKeys.SkillsAll, "PUBLISHED");

                if (response.JobProfileSkills != null && skillsResponse.Skill != null)
                {
                    SkillsResponse jobProfileSkillsResponse = new SkillsResponse();
                    List<Skills> jobProfileSkillsList = new List<Skills>();

                    var filteredSkills = response.JobProfileSkills.SelectMany(d => d.Relatedskills.ContentItems).ToList();

                    foreach (var skill in skillsResponse.Skill)
                    {
                        if (skill.DisplayText != null && filteredSkills.Any(d => d.RelatedSkillDesc.Equals(skill.DisplayText)))
                        {
                            jobProfileSkillsList.Add(skill);
                        }
                    }

                    jobProfileSkillsResponse.Skill = jobProfileSkillsList;

                    var mappedResponse = mapper.Map<JobProfileSkillSegmentDataModel>(response);
                    List<JobProfSkills> sortedSkills = new List<JobProfSkills>();
                    var mappedSkillsResponse = mapper.Map<List<OnetSkill>>(jobProfileSkillsResponse.Skill);
                    var mappedContextualSkills = mapper.Map<List<ContextualisedSkill>>(filteredSkills);

                    foreach (var skill in filteredSkills)
                    {
                        sortedSkills.Add(new JobProfSkills
                        {
                            ContextualisedSkill = mappedContextualSkills.Single(s => s.Description == skill.RelatedSkillDesc),
                            OnetSkill = mappedSkillsResponse.Single(s => s.Title == skill.RelatedSkillDesc),
                        });
                    }

                    mappedResponse.Skills = sortedSkills;

                    var skillsObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Skills/Body.cshtml", mappedResponse).ConfigureAwait(false);

                    skills = new SegmentModel
                    {
                        Segment = JobProfileSegment.WhatItTakes,
                        JsonV1 = skillsObject,
                        RefreshStatus = RefreshStatus.Success,
                        Markup = new HtmlString(html),
                    };
                }
            }
            catch (Exception ex)
            {
                logService.LogError(ex.ToString());
                throw;
            }

            return skills;
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