﻿using AutoMapper;
using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Models.Segment.CareerPath;
using DFC.App.JobProfile.Data.Models.Segment.CurrentOpportunities;
using DFC.App.JobProfile.Data.Models.Segment.HowToBecome;
using DFC.App.JobProfile.Data.Models.Segment.Overview;
using DFC.App.JobProfile.Data.Models.Segment.RelatedCareers;
using DFC.App.JobProfile.Data.Models.Segment.SkillsModels;
using DFC.App.JobProfile.Data.Models.Segment.Tasks;
using DFC.App.JobProfile.ProfileService.Utilities;
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
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobProfSkills = DFC.App.JobProfile.Data.Models.Segment.SkillsModels.Skills;
using Skills = DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.JobProfiles.Skills;

namespace DFC.App.JobProfile.ProfileService
{
    public class JobProfileService : IJobProfileService
    {
        private const string SegementExpiryAppSettings = "Cms:SegementExpiry";
        private const string CoursesApprenticeshipExpiryAppSettings = "Cms:CoursesApprenticeshipExpiry";
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogService logService;
        private readonly ISharedContentRedisInterface sharedContentRedisInterface;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        private readonly ICourseSearchApiService client;
        private readonly IAVAPIService avAPIService;
        private readonly string howToBecomeOfflineMarkup;
        private readonly string overviewOfflineMarkup;
        private readonly string currentOpportunitiesOfflineMarkup;
        private readonly string relatedCareersOfflineMarkup;
        private readonly string whatItTakesOfflineMarkup;
        private readonly string whatYouWillDoOfflineMarkup;
        private readonly string careerPathOfflineMarkup;
        private readonly string filter;
        private double segementExpiryInHours = 24;
        private double coursesApprenticeshipExpiryInHours = 48;

        public JobProfileService(
            IMapper mapper,
            ILogService logService,
            ISharedContentRedisInterface sharedContentRedisInterface,
            IRazorTemplateEngine razorTemplateEngine,
            IConfiguration configuration,
            ICourseSearchApiService client,
            IAVAPIService avAPIService)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.logService = logService;
            this.sharedContentRedisInterface = sharedContentRedisInterface;
            this.razorTemplateEngine = razorTemplateEngine;
            this.client = client;
            this.avAPIService = avAPIService;
            filter = configuration.GetSection("contentMode:contentMode").Get<string>() ?? "PUBLISHED";
            howToBecomeOfflineMarkup = configuration.GetSection("HowToBecomeSegmentClientOptions:OfflineHtml").Get<string>();
            relatedCareersOfflineMarkup = configuration.GetSection("RelatedCareersSegmentClientOptions:OfflineHtml").Get<string>();
            overviewOfflineMarkup = configuration.GetSection("OverviewBannerSegmentClientOptions:OfflineHtml").Get<string>();
            currentOpportunitiesOfflineMarkup = configuration.GetSection("CurrentOpportunitiesSegmentClientOptions:OfflineHtml").Get<string>();
            whatItTakesOfflineMarkup = configuration.GetSection("WhatItTakesSegmentClientOptions:OfflineHtml").Get<string>();
            whatYouWillDoOfflineMarkup = configuration.GetSection("WhatYouWillDoSegmentClientOptions:OfflineHtml").Get<string>();
            careerPathOfflineMarkup = configuration.GetSection("CareerPathSegmentClientOptions:OfflineHtml").Get<string>();

            if (this.configuration != null)
            {
                string segementExpiryAppString = this.configuration.GetSection(SegementExpiryAppSettings).Get<string>();
                if (double.TryParse(segementExpiryAppString, out var segementExpiryAppStringParseResult))
                {
                    segementExpiryInHours = segementExpiryAppStringParseResult;
                }

                string coursesApprenticeshipExpiryAppString = this.configuration.GetSection(CoursesApprenticeshipExpiryAppSettings).Get<string>();
                if (double.TryParse(coursesApprenticeshipExpiryAppString, out var coursesApprenticeshipExpiryAppStringParseResult))
                {
                    coursesApprenticeshipExpiryInHours = coursesApprenticeshipExpiryAppStringParseResult;
                }
            }
        }

        public async Task<IEnumerable<JobProfileModel>> GetAllAsync()
        {
            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesResponse>(ApplicationKeys.JobProfileCurrentOpportunitiesAllJobProfiles, filter, segementExpiryInHours);

                return mapper.Map<IEnumerable<JobProfileModel>>(response.JobProfileCurrentOpportunities);
            }
            catch (Exception ex)
            {
                logService.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<JobProfileModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            var data = new JobProfileModel()
            {
                CanonicalName = canonicalName,
                BreadcrumbTitle = new CultureInfo("en-GB").TextInfo.ToTitleCase(canonicalName),
                Segments = new List<SegmentModel>()
            };

            var requiredSegments = new[]
            {
                JobProfileSegment.Overview,
                JobProfileSegment.HowToBecome,
                JobProfileSegment.WhatItTakes,
            };

            try
            {
                var overviewTask = GetOverviewSegment(canonicalName, filter);
                var howToBecomeTask = GetHowToBecomeSegmentAsync(canonicalName, filter);
                var skillsTask = GetSkillSegmentAsync(canonicalName, filter);

                Task<SegmentModel>[] requiredTasks =
                {
                    overviewTask,
                    howToBecomeTask,
                    skillsTask,
                };

                await Task.WhenAll(requiredTasks);

                if (requiredTasks.All(t => t.IsCompletedSuccessfully))
                {
                    data.Segments.Add(await overviewTask);
                    data.Segments.Add(await howToBecomeTask);
                    data.Segments.Add(await skillsTask);
                }

                if (data.Segments.Any(segment =>
                        requiredSegments.Contains(segment.Segment) &&
                        segment.RefreshStatus == RefreshStatus.Failed))
                {
                    return null;
                }

                var tasksTask = GetTasksSegmentAsync(canonicalName, filter);
                var careersPathTask = GetCareerPathSegmentAsync(canonicalName, filter);
                var currentOpportunityTask = GetCurrentOpportunities(canonicalName);
                var relatedCareersTask = GetRelatedCareersSegmentAsync(canonicalName, filter);
                var videoTask = GetSocialProofVideoSegment(canonicalName, filter);

                Task<SegmentModel>[] optionalTasks =
                {
                    relatedCareersTask,
                    careersPathTask,
                    tasksTask,
                    currentOpportunityTask,
                };

                await Task.WhenAll(optionalTasks);

                if (optionalTasks.All(t => t.IsCompletedSuccessfully))
                {
                    data.Segments.Add(await tasksTask);
                    data.Segments.Add(await careersPathTask);
                    data.Segments.Add(await currentOpportunityTask);
                    data.Segments.Add(await relatedCareersTask);
                    data.Video = await videoTask;
                }

                // Retrieves the title from the Overview segment and assign to MetaTags.Title
                var overviewData = System.Text.Json.JsonSerializer.Deserialize<OverviewApiModel>(overviewTask.Result.JsonV1, 
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });

                data.MetaTags = new MetaTags()
                {
                    Title = overviewData.Title
                };

                return data;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }
        }

        /// <summary>
        /// Get RelatedCareers from STAX via GraphQl for a job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile URL.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>RelatedCareers segment model.</returns>
        public async Task<SegmentModel> GetRelatedCareersSegmentAsync(string canonicalName, string filter)
        {
            SegmentModel relatedCareers = new()
            {
                Segment = JobProfileSegment.RelatedCareers,
                Markup = new HtmlString(relatedCareersOfflineMarkup),
            };
            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(ApplicationKeys.JobProfileRelatedCareersPrefix + "/" + canonicalName, filter, segementExpiryInHours);

                if (response != null && response.JobProfileRelatedCareers.IsAny())
                {

                    var mappedResponse = mapper.Map<RelatedCareerSegmentDataModel>(response);

                    var relatedCareersObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/RelatedCareers/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        relatedCareers.Markup = new HtmlString(html);
                        relatedCareers.JsonV1 = relatedCareersObject;
                        relatedCareers.RefreshStatus = RefreshStatus.Success;
                    }
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
            SegmentModel howToBecome = new()
            {
                Segment = JobProfileSegment.HowToBecome,
                Markup = new HtmlString(howToBecomeOfflineMarkup),
            };

            try
            {
                // Get the response from GraphQl
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(ApplicationKeys.JobProfileHowToBecome + "/" + canonicalName, filter, segementExpiryInHours);

                if (response != null && response.JobProfileHowToBecome.IsAny())
                {
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

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        howToBecome.Markup = new HtmlString(html);
                        howToBecome.JsonV1 = howToBecomeObject;
                        howToBecome.RefreshStatus = RefreshStatus.Success;
                    }
                }

                return howToBecome;
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
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
            SegmentModel currentOpportunities = new()
            {
                Segment = JobProfileSegment.CurrentOpportunities,
                Markup = new HtmlString(currentOpportunitiesOfflineMarkup),
            };

            try
            {
                var currentOpportunitiesSegmentModel = new CurrentOpportunitiesSegmentModel();
                currentOpportunitiesSegmentModel.Data = new CurrentOpportunitiesSegmentDataModel();
                currentOpportunitiesSegmentModel.Data.Courses = new Courses();
                currentOpportunitiesSegmentModel.CanonicalName = canonicalName;

                //Get job profile course keyword and lars code
                var jobProfile = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCurrentOpportunitiesGetbyUrlReponse>(string.Concat(ApplicationKeys.JobProfileCurrentOpportunities, "/", canonicalName), filter, segementExpiryInHours);

                //get courses by course key words
                if (jobProfile != null && jobProfile.JobProfileCurrentOpportunitiesGetByUrl.IsAny())
                {
                    string jobTitle = jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].DisplayText;
                    currentOpportunitiesSegmentModel.Data.TitlePrefix = jobTitle.AddPrefix();
                    currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords = string.Empty;

                    var opportunities = new List<Opportunity>();
                    if (!string.IsNullOrEmpty(jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].Coursekeywords))
                    {
                        string coursekeywords = jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].Coursekeywords;
                        var results = await GetCourses(coursekeywords, canonicalName);
                        var courseSearchResults = results.Courses?.ToList();

                        if (courseSearchResults != null)
                        {
                            opportunities = MapCourses(courseSearchResults, opportunities);
                        }

                        currentOpportunitiesSegmentModel.Data.Courses.CourseKeywords = coursekeywords;
                    }

                    currentOpportunitiesSegmentModel.Data.Courses.Opportunities = opportunities;

                    currentOpportunitiesSegmentModel.Data.Apprenticeships = new Apprenticeships();
                    currentOpportunitiesSegmentModel.Data.Apprenticeships.Vacancies = new List<Vacancy>();

                    //get apprenticeship by lars code.
                    if (jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode?.ContentItems.Length > 0 &&
                        jobProfile.JobProfileCurrentOpportunitiesGetByUrl[0].SOCCode?.ContentItems?[0].ApprenticeshipStandards.ContentItems.Length > 0)
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

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        logService.LogInformation($"{nameof(GetCurrentOpportunities)} HTML has been rendered.");

                        currentOpportunities.Markup = new HtmlString(html);
                        currentOpportunities.JsonV1 = currentOpportunitiesObject;
                        currentOpportunities.RefreshStatus = RefreshStatus.Success;
                    }
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            return currentOpportunities;
        }

        /// <summary>
        /// Get Overview Segment data from NuGet packages.
        /// </summary>
        /// <param name="canonicalName">Job profile name.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>Overview Segment model.</returns>
        public async Task<SegmentModel> GetOverviewSegment(string canonicalName, string filter)
        {
            SegmentModel overview = new()
            {
                Segment = JobProfileSegment.Overview,
                Markup = new HtmlString(overviewOfflineMarkup),
            };

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(string.Concat(ApplicationKeys.JobProfileOverview, "/", canonicalName), filter, segementExpiryInHours);

                if (response != null && response.JobProfileOverview.IsAny())
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

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        overview.Markup = new HtmlString(html);
                        overview.JsonV1 = overviewObject;
                        overview.RefreshStatus = RefreshStatus.Success;
                    }
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            return overview;
        }

        /// <summary>
        /// Get What Youll Do from STAX via GraphQl for a job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile URL.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>WhatYoullDo segment model.</returns>
        public async Task<SegmentModel> GetTasksSegmentAsync(string canonicalName, string filter)
        {
            SegmentModel tasks = new()
            {
                Segment = JobProfileSegment.WhatYouWillDo,
                Markup = new HtmlString(whatYouWillDoOfflineMarkup),
            };

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(ApplicationKeys.JobProfileWhatYoullDo + "/" + canonicalName, filter, segementExpiryInHours);

                if (response != null)
                {
                    var mappedResponse = mapper.Map<TasksSegmentDataModel>(response);

                    var tasksObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy(),
                        },
                    });
                    
                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/Tasks/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        tasks.Markup = new HtmlString(html);
                        tasks.JsonV1 = tasksObject;
                        tasks.RefreshStatus = RefreshStatus.Success;
                    }
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            return tasks;
        }

        /// <summary>
        /// Get CareerPath from STAX via GraphQl for a job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile URL.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>CareerPath segment model.</returns>
        public async Task<SegmentModel> GetCareerPathSegmentAsync(string canonicalName, string filter)
        {
            SegmentModel careerPath = new()
            {
                Segment = JobProfileSegment.CareerPathsAndProgression,
                Markup = new HtmlString(careerPathOfflineMarkup),
            };

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(ApplicationKeys.JobProfileCareerPath + "/" + canonicalName, filter, segementExpiryInHours);

                if (response != null && response.JobProileCareerPath != null)
                {
                    var mappedResponse = mapper.Map<CareerPathSegmentDataModel>(response);

                    var careerPathObject = JsonConvert.SerializeObject(mappedResponse, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() } });

                    var html = await razorTemplateEngine.RenderAsync("~/Views/Profile/Segment/CareerPath/BodyData.cshtml", mappedResponse).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        careerPath.Markup = new HtmlString(html);
                        careerPath.JsonV1 = careerPathObject;
                        careerPath.RefreshStatus = RefreshStatus.Success;
                    }
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
        /// <param name="filter"> Contains the contentMode variable value used to determine whether to retrieve data from draft or published on graphQL.</param>
        /// <returns>Returns segment information containing HTML markup data to render the "What it Takes" segment.</returns>
        public async Task<SegmentModel> GetSkillSegmentAsync(string canonicalName, string filter)
        {
            SegmentModel skills = new()
            {
                Segment = JobProfileSegment.WhatItTakes,
                Markup = new HtmlString(whatItTakesOfflineMarkup),
            };

            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(ApplicationKeys.JobProfileSkillsSuffix + "/" + canonicalName, filter, segementExpiryInHours);
                var skillsResponse = await sharedContentRedisInterface.GetDataAsyncWithExpiry<SkillsResponse>(ApplicationKeys.SkillsAll, "PUBLISHED", segementExpiryInHours);

                if (response != null && response.JobProfileSkills != null && skillsResponse.Skill != null)
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

                    if (!string.IsNullOrWhiteSpace(html))
                    {
                        skills.Markup = new HtmlString(html);
                        skills.JsonV1 = skillsObject;
                        skills.RefreshStatus = RefreshStatus.Success;
                    }
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            return skills;
        }

        /// <summary>
        /// Get SocialProofVideo from STAX via GraphQl for a job profile.
        /// </summary>
        /// <param name="canonicalName">Job profile URL.</param>
        /// <param name="filter">PUBLISHED or DRAFT.</param>
        /// <returns>SocialProofVideo segment model.</returns>
        public async Task<SocialProofVideo> GetSocialProofVideoSegment(string canonicalName, string filter)
        {
            try
            {
                var response = await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileVideoResponse>(string.Concat(ApplicationKeys.JobProfileVideoPrefix, "/", canonicalName), filter, segementExpiryInHours);

                if (response != null && response.JobProfileVideo.IsAny() && response.JobProfileVideo[0].VideoType != null && response.JobProfileVideo[0].VideoType != "None")
                {
                    return mapper.Map<SocialProofVideo>(response);
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
                throw;
            }

            return null;
        }

        /// <summary>
        /// Refresh all courses in Redis.
        /// </summary>
        /// <returns>boolean.</returns>
        /// <exception cref="ArgumentNullException">throw exception when jobprofile data is null.</exception>
        public async Task<bool> RefreshCourses(string filter, int first, int skip)
        {
            bool returndata = true;
            int total = skip + first;

            //Get job profile with Url name
            var jobProfile = await sharedContentRedisInterface.GetDataAsyncWithExpiryAndFirstSkip<JobProfileCurrentOpportunitiesResponse>(ApplicationKeys.JobProfileCurrentOpportunitiesAllJobProfiles + "/" + skip + "-" + total, filter, first, skip, segementExpiryInHours);

            if (jobProfile != null && jobProfile.JobProfileCurrentOpportunities != null)
            {
                if (jobProfile.JobProfileCurrentOpportunities.Any())
                {
                    foreach (var each in jobProfile.JobProfileCurrentOpportunities)
                    {
                        string canonicalName = each.PageLocation.UrlName;

                        string courseKeywords = each.Coursekeywords;
                        if (!string.IsNullOrEmpty(courseKeywords))
                        {
                            string cacheKey = ApplicationKeys.JobProfileCurrentOpportunitiesCoursesPrefix + '/' + canonicalName + '/' + courseKeywords.ConvertCourseKeywordsString();
                            await GetCoursesAndCachedRedis(courseKeywords, cacheKey);
                        }
                    }
                }
            }
            else
            {
                logService.LogError("Refresh Courses error: Job profiles is null.");
                throw new ArgumentNullException("Refresh Courses error: Job profiles is null.");
            }

            return returndata;
        }

        /// <summary>
        /// Refresh all segments redis.
        /// </summary>
        /// <param name="filter">PUBLISHED</param>
        /// <returns>boolean.</returns>
        /// <exception cref="ArgumentNullException">throw exception when jobprofile data is null.</exception>
        public async Task<bool> RefreshAllSegments(string filter, int first, int skip)
        {
            bool returndata = true;
            int total = skip + first;

            //Get job profile with Url name
            var jobProfile = await sharedContentRedisInterface.GetDataAsyncWithExpiryAndFirstSkip<JobProfileCurrentOpportunitiesResponse>(ApplicationKeys.JobProfileCurrentOpportunitiesAllJobProfiles + "/" + skip + "-" + total, filter, first, skip, segementExpiryInHours);

            if (jobProfile != null && jobProfile.JobProfileCurrentOpportunities != null)
            {
                logService.LogInformation($"{nameof(RefreshAllSegments)} data retrieved from Redis.");

                if (jobProfile.JobProfileCurrentOpportunities.Count() > 0)
                {
                    foreach (var each in jobProfile.JobProfileCurrentOpportunities)
                    {
                        string canonicalName = each.PageLocation.UrlName;

                        //Refresh Overview
                        string overviewCacheKey = string.Concat(ApplicationKeys.JobProfileOverview, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(overviewCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfilesOverviewResponse>(overviewCacheKey, filter, segementExpiryInHours);

                        //Refresh RelatedCareers
                        string relatedCareersCacheKey = string.Concat(ApplicationKeys.JobProfileRelatedCareersPrefix, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(relatedCareersCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<RelatedCareersResponse>(relatedCareersCacheKey, filter, segementExpiryInHours);

                        //Refresh WhatYoullDo
                        string whatYoullDoCacheKey = string.Concat(ApplicationKeys.JobProfileWhatYoullDo, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(whatYoullDoCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileWhatYoullDoResponse>(whatYoullDoCacheKey, filter, segementExpiryInHours);

                        //Refresh CareerPath
                        string careerPathCacheKey = string.Concat(ApplicationKeys.JobProfileCareerPath, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(careerPathCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileCareerPathAndProgressionResponse>(careerPathCacheKey, filter, segementExpiryInHours);

                        //Refresh Skill
                        string skillCacheKey = string.Concat(ApplicationKeys.JobProfileSkillsSuffix, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(skillCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileSkillsResponse>(skillCacheKey, filter, segementExpiryInHours);

                        //Refresh HowToBecome
                        string howToBecomeCacheKey = string.Concat(ApplicationKeys.JobProfileHowToBecome, "/", canonicalName);
                        await sharedContentRedisInterface.InvalidateEntityAsync(howToBecomeCacheKey, filter);
                        await sharedContentRedisInterface.GetDataAsyncWithExpiry<JobProfileHowToBecomeResponse>(howToBecomeCacheKey, filter, segementExpiryInHours);
                    }
                }
            }
            else
            {
                logService.LogError("Refresh All segments failed, because job profile is null.");

                throw new ArgumentNullException("Refresh All segments failed, because job profile is null.");
            }

            return returndata;
        }

        public async Task<bool> RefreshApprenticeshipsAsync(string filter, int first, int skip)
        {
            bool returndata = true;
            int total = skip + first;

            //Get job profile with Url name
            var jobProfile = await sharedContentRedisInterface.GetDataAsyncWithExpiryAndFirstSkip<JobProfileCurrentOpportunitiesResponse>(ApplicationKeys.JobProfileCurrentOpportunitiesAllJobProfiles + "/" + skip + "-" + total, filter, first, skip, segementExpiryInHours);

            if (jobProfile != null && jobProfile.JobProfileCurrentOpportunities.Count() > 0)
            {
                foreach (var each in jobProfile.JobProfileCurrentOpportunities)
                {
                    var larsCodes = each.SOCCode.ContentItems?.SelectMany(x => x.ApprenticeshipStandards.ContentItems).Select(x => x.LARScode).ToList();

                    if (larsCodes != null && larsCodes.Count > 0)
                    {
                        string cachekey = string.Concat(ApplicationKeys.JobProfileCurrentOpportunitiesAVPrefix, "/", each.PageLocation.UrlName, "/", string.Join(",", larsCodes.OrderBy(x => x)));
                        await GetApprenticeshipsAndCachedRedisAsync(larsCodes, cachekey);
                    }
                }
            }

            return returndata;
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
        /// <param name="canonicalName">Job profile url name</param>
        /// <returns>CourseResponse contains list Courses.</returns>
        private async Task<CoursesResponse> GetCourses(string courseKeywords, string canonicalName)
        {
            string cacheKey = ApplicationKeys.JobProfileCurrentOpportunitiesCoursesPrefix + '/' + canonicalName + '/' + courseKeywords.ConvertCourseKeywordsString();
            var redisData = await sharedContentRedisInterface.GetCurrentOpportunitiesData<CoursesResponse>(cacheKey);

            if (redisData == null)
            {
                redisData = new CoursesResponse();
                try
                {
                    redisData = await GetCoursesAndCachedRedis(courseKeywords, cacheKey);
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
        /// <returns> List of mapped Opportunity objects. </returns>
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
        /// <param name="canonicalName">Job profile url name</param>
        /// <returns>IEnumerable of Vacancy.</returns>
        private async Task<IEnumerable<Vacancy>> GetApprenticeshipVacanciesAsync(List<string> larsCodes, string canonicalName)
        {
            // Add LARS code to cache key
            string cacheKey = ApplicationKeys.JobProfileCurrentOpportunitiesAVPrefix + '/' + canonicalName + '/' + string.Join(",", larsCodes.OrderBy(x => x));
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

        /// <summary>
        /// Get courses from API and save to Redis.
        /// </summary>
        /// <param name="courseKeywords">course search key words.</param>
        /// <param name="cacheKey">Redis cache key.</param>
        /// <returns>courses list.</returns>
        private async Task<CoursesResponse> GetCoursesAndCachedRedis(string courseKeywords, string cacheKey)
        {
            var redisData = new CoursesResponse();
            try
            {
                var result = await client.GetCoursesAsync(courseKeywords, true).ConfigureAwait(false);
                redisData.Courses = result?.ToList();
                var save = await sharedContentRedisInterface.SetCurrentOpportunitiesData<CoursesResponse>(redisData, cacheKey, coursesApprenticeshipExpiryInHours);

                if (!save)
                {
                    logService.LogError("Redis failed: Course Keywords-" + courseKeywords + " Cache Key-" + cacheKey);
                }
                else
                {
                    logService.LogInformation("Redis saved: Course Keywords-" + courseKeywords + " Cache Key-" + cacheKey);
                }
            }
            catch (Exception ex)
            {
                logService.LogError(ex.ToString());
            }

            return redisData;
        }

        private async Task GetApprenticeshipsAndCachedRedisAsync(List<string> larsCodes, string cacheKey)
        {
            try
            {
                var avMapping = new AVMapping { Standards = larsCodes };
                var avResponse = await avAPIService.GetAVsForMultipleProvidersAsync(avMapping).ConfigureAwait(false);
                var mappedAVResponse = mapper.Map<IEnumerable<Vacancy>>(avResponse);
                var vacancies = mappedAVResponse.Take(2).ToList();

                if (vacancies.Any())
                {
                    var save = await sharedContentRedisInterface.SetCurrentOpportunitiesData(vacancies, cacheKey, coursesApprenticeshipExpiryInHours);
                    if (!save)
                    {
                        throw new InvalidOperationException("Redis save process failed.");
                    }
                    else
                    {
                        logService.LogInformation($"Redis saved: Apprenticeship cache key: {cacheKey}.");
                    }
                }
                else
                {
                    logService.LogInformation($"Redis not saved: Apprenticeship cache key: {cacheKey}.  No vacancies found.");
                }
            }
            catch (Exception exception)
            {
                logService.LogError(exception.ToString());
            }
        }
    }
}
