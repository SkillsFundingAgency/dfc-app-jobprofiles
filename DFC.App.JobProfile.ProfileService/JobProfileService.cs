﻿using AutoMapper;
using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.CareerPath;
using DFC.App.JobProfile.Data.Models.CurrentOpportunities;
using DFC.App.JobProfile.Data.Models.Overview;
using DFC.App.JobProfile.Data.Models.RelatedCareersModels;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Data.Models.Segment.Tasks;
using DFC.App.JobProfile.Data.Models.SkillsModels;
using DFC.Common.SharedContent.Pkg.Netcore.Constant;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.Response;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JobProfSkills = DFC.App.JobProfile.Data.Models.SkillsModels.Skills;
using Skills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Skills;

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
        private readonly IAVAPIService avAPIService;
        private string status = string.Empty;

        public JobProfileService(
            ICosmosRepository<JobProfileModel> repository,
            ISegmentService segmentService,
            IMapper mapper,
            ILogService logService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IRazorTemplateEngine razorTemplateEngine,
            IConfiguration configuration,
            ICourseSearchApiService client,
            IAVAPIService avAPIService)
        {
            this.repository = repository;
            this.segmentService = segmentService;
            this.mapper = mapper;
            this.logService = logService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
            this.client = client;
            this.avAPIService = avAPIService;
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

            try
            {
                var howToBecomeTask = GetHowToBecomeSegmentAsync(canonicalName, status);
                var overviewTask = GetOverviewSegment(canonicalName, status);
                var relatedCareersTask = GetRelatedCareersSegmentAsync(canonicalName, status);
                var careersPathTask = GetCareerPathSegmentAsync(canonicalName, status);
                var skillsTask = GetSkillSegmentAsync(canonicalName, status);
                var videoTask = GetSocialProofVideoSegment(canonicalName, status);
                var tasksTask = GetTasksSegmentAsync(canonicalName, status);
                var currentOpportunityTask = GetCurrentOpportunities(canonicalName);

                await Task.WhenAll(howToBecomeTask, overviewTask, relatedCareersTask, careersPathTask, skillsTask, videoTask, tasksTask, currentOpportunityTask);

                var data = new JobProfileModel()
                {
                    CanonicalName = canonicalName,
                    BreadcrumbTitle = new CultureInfo("en-GB").TextInfo.ToTitleCase(canonicalName),
                    Segments = new List<SegmentModel>(),
                };

                if (howToBecomeTask.IsCompletedSuccessfully && overviewTask.IsCompletedSuccessfully && relatedCareersTask.IsCompletedSuccessfully &&
                    careersPathTask.IsCompletedSuccessfully && skillsTask.IsCompletedSuccessfully && videoTask.IsCompletedSuccessfully &&
                    tasksTask.IsCompletedSuccessfully && currentOpportunityTask.IsCompletedSuccessfully)
                {
                    data.Segments.Add(await howToBecomeTask);
                    data.Segments.Add(await relatedCareersTask);
                    data.Segments.Add(await overviewTask);
                    data.Segments.Add(await currentOpportunityTask);
                    data.Segments.Add(await skillsTask);
                    data.Segments.Add(await careersPathTask);
                    data.Segments.Add(await tasksTask);
                    data.Video = await videoTask;
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

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/RelatedCareers/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

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

        /// <summary>
        /// Get HowToBecome from STAX via GraphQl for a job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile URL.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>HowToBecome segment model.</returns>
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
                var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/HowToBecome/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

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
        /// Get current opportunities data for individual job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile url.</param>
        /// <returns>Current Opportunities Segment model.</returns>
        public async Task<SegmentModel> GetCurrentOpportunities(string canonicalName)
        {
            var currentOpportunities = new SegmentModel() { Segment = JobProfileSegment.CurrentOpportunities };
            var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel();
            currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentDataModel();
            currentOpportunitiesSegmentModel.Data.Courses = new Courses();
            currentOpportunitiesSegmentModel.CanonicalName = canonicalName;

            //Get job profile course keyword and lars code
            var jobProfile = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>(string.Concat(ApplicationKeys.JobProfileCurrentOpportunitiesCoursesPrefix, "/", canonicalName), "PUBLISHED");

            //get courses by course key words
            if (jobProfile.JobProfileCurrentOpportunitiesGetByUrl != null && jobProfile.JobProfileCurrentOpportunitiesGetByUrl.Any())
            {
                string coursekeywords = jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].Coursekeywords;
                string jobTitle = jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].DisplayText;
                var results = await GetCourses(coursekeywords, canonicalName);
                var courseSearchResults = results.Courses?.ToList();

                var opportunities = new List<Opportunity>();
                if (courseSearchResults != null)
                {
                    opportunities = MapCourses(courseSearchResults, opportunities);
                }

                currentOpportunitiesSegmentModel.Data.TitlePrefix = AddPrefix(jobTitle);
                currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords = coursekeywords;
                currentOpportunitiesSegmentModel.Data.Courses.Opportunities = opportunities;
                currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships();

                //get apprenticeship by lars code.
                if (jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode?.ContentItems.Length > 0 && jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode?.ContentItems?[0].ApprenticeshipStandards.ContentItems.Length > 0)
                {
                    if (!string.IsNullOrEmpty(jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode?.ContentItems?[0].ApprenticeshipStandards.ContentItems?[0].LARScode))
                    {
                        var larsCodes = jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode.ContentItems
                            .SelectMany(socCode => socCode.ApprenticeshipStandards.ContentItems
                            .Select(standard => standard.LARScode)).ToList();
                        var apprenticeshipVacancies = await GetApprenticeshipVacanciesAsync(larsCodes, canonicalName);
                        currentOpportunitiesSegmentModel.Data.Apprenticeships.Vacancies = apprenticeshipVacancies;
                    }
                }

                currentOpportunitiesSegmentModel.Data.JobTitle = jobTitle;

                var currentOpportunitiesObject = JsonConvert.SerializeObject(currentOpportunitiesSegmentModel.Data, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/CurrentOpportunities/BodyData.cshtml", currentOpportunitiesSegmentModel.Data).ConfigureAwait(false);

                currentOpportunities = new SegmentModel
                {
                    Segment = JobProfileSegment.CurrentOpportunities,
                    JsonV1 = currentOpportunitiesObject,
                    RefreshStatus = RefreshStatus.Success,
                    Markup = new HtmlString(html),
                };
            }

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
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(string.Concat(ApplicationKeys.JobProfileOverview, "/", canonicalName), filter);

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

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/Overview/BodyData.cshtml", mappedOverview).ConfigureAwait(false);

                    overview = new SegmentModel
                    {
                        Segment = JobProfileSegment.Overview,
                        JsonV1 = overviewObject,
                        RefreshStatus = RefreshStatus.Success,
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

        public async Task<SegmentModel> GetTasksSegmentAsync(string canonicalName, string filter)
        {
            var tasks = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(ApplicationKeys.JobProfileWhatYoullDo + "/" + canonicalName, filter);

                var mappedResponse = mapper.Map<TasksSegmentDataModel>(response);

                var tasksObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },
                });

                var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/Tasks/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

                tasks = new SegmentModel
                {
                    Segment = JobProfileSegment.WhatYouWillDo,
                    Markup = new HtmlString(html),
                    JsonV1 = tasksObject,
                    RefreshStatus = RefreshStatus.Success,
                };
            }
            catch (Exception e)
            {
                logService.LogError(e.ToString());
            }

            return tasks;
        }

        public async Task<SegmentModel> GetCareerPathSegmentAsync(string canonicalName, string status)
        {
            SegmentModel careerPath = new SegmentModel();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(ApplicationKeys.JobProfileCareerPath + "/" + canonicalName, status);

                if (response.JobProileCareerPath != null)
                {
                    var mappedResponse = mapper.Map<CareerPathSegmentDataModel>(response);

                    var careerPathObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/CareerPath/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

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

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/Skills/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

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

        public async Task<SocialProofVideo> GetSocialProofVideoSegment(string canonicalName, string filter)
        {
            SocialProofVideo mappedVideo = new SocialProofVideo();

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(string.Concat(ApplicationKeys.JobProfileVideoPrefix, "/", canonicalName), filter);

                if (response != null)
                {
                    if (response.JobProfileVideo != null && response.JobProfileVideo.Count > 0 && response.JobProfileVideo.FirstOrDefault().VideoType != null)
                    {
                        mappedVideo = mapper.Map<SocialProofVideo>(response);
                    }
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
            }

            return mappedVideo;
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

        private static string AddPrefix(string jobTitle)
        {
            if (string.IsNullOrEmpty(jobTitle))
            {
                return "a";
            }
            else if ("AEIOUaeiou".IndexOf(jobTitle[0]) != -1)
            {
                return "an";
            }
            else
            {
                return "a";
            }
        }

        private static string ConvertCourseKeywordsString(string input)
        {
            // Regular expression pattern to match substrings within single quotes
            string pattern = @"'([^']*)'";

            // Find all matches of substrings within single quotes, extract substrings from matches, join by a comma and convert to a string
            var result = string.Join(",", Regex.Matches(input, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(1))
                .OfType<Match>()
                .Select(m => m.Groups[1].Value));

            return result;
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

        /// <summary>
        /// Get Courses from FAC client API.
        /// </summary>
        /// <param name="courseKeywords">Courses key words, such as 'building services engineering'.</param>
        /// <returns>CourseResponse contains list Courses.</returns>
        private async Task<CoursesReponse> GetCourses(string courseKeywords, string canonicalName)
        {
            string cacheKey = ApplicationKeys.JobProfileCurrentOpportunitiesCoursesPrefix + '/' + canonicalName + '/' + ConvertCourseKeywordsString(courseKeywords);
            var redisData = await sharedContentRedisInterface.GetCurrentOpportunitiesData<CoursesReponse>(cacheKey);
            if (redisData == null)
            {
                redisData = new CoursesReponse();
                try
                {
                    var result = await client.GetCoursesAsync(courseKeywords, true).ConfigureAwait(false);

                    redisData.Courses = result.ToList();

                    var save = await sharedContentRedisInterface.SetCurrentOpportunitiesData<CoursesReponse>(redisData, cacheKey, 48);
                    if (!save)
                    {
                        throw new InvalidOperationException("Redis save process failed.");
                    }
                }
                catch (Exception ex)
                {
                    logService.LogError(ex.ToString());
                }
            }

            return redisData;
        }

        /// <summary>
        /// Mapping courses data with Opportunity object.
        /// </summary>
        /// <param name="courseSearchResults">List of courses result data.</param>
        /// <param name="opportunities">List of Opportunity object.</param>
        /// <returns> List of Opportunity object. </returns>
        private List<Opportunity> MapCourses(List<Course> courseSearchResults, List<Opportunity> opportunities)
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

        /// <summary>
        /// Get apprenticeship vacancies from Apprenticeship API.
        /// </summary>
        /// <param name="larsCodes">List of LARS codes.</param>
        /// <returns>IEnumerable of Vacancy.</returns>
        private async Task<IEnumerable<Vacancy>> GetApprenticeshipVacanciesAsync(List<string> larsCodes, string canonicalName)
        {
            // Add LARS code to cache key
            string cacheKey = ApplicationKeys.JobProfileCurrentOpportunitiesAVPrefix + '/' + canonicalName + '/' + string.Join(",", larsCodes);
            var redisData = await sharedContentRedisInterface.GetCurrentOpportunitiesData<List<Vacancy>>(cacheKey);
            var avMapping = new AVMapping { Standards = larsCodes };

            // If there are no apprenticeship vacancies data in Redis then get data from the Apprenticeship Vacancy API
            if (redisData == null)
            {
                try
                {
                    // Get apprenticeship vacancies from Apprenticeship API
                    var avResponse = await avAPIService.GetAVsForMultipleProvidersAsync(avMapping).ConfigureAwait(false);

                    // Map list of vacancies to IEnumerable<Vacancy>
                    var mappedAVResponse = mapper.Map<IEnumerable<Vacancy>>(avResponse);

                    var vacancies = mappedAVResponse.Take(2).ToList();

                    // Save data to Redis
                    var save = await sharedContentRedisInterface.SetCurrentOpportunitiesData(vacancies, cacheKey, 48);
                    if (!save)
                    {
                        throw new InvalidOperationException("Redis save process failed.");
                    }

                    return vacancies;
                }
                catch (Exception e)
                {
                    logService.LogError(e.ToString());
                }
            }

            return redisData;
        }
    }
}
