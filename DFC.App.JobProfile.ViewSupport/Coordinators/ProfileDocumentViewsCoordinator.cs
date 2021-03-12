using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Providers;
using DFC.App.JobProfile.ViewSupport.Configuration;
using DFC.App.JobProfile.ViewSupport.Models;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using DFC.App.Services.Common.Factories;
using DFC.App.Services.Common.Faults;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Coordindators
{
    internal sealed class ProfileDocumentViewsCoordinator :
        ICoordinateProfileDocumentViews,
        IRequireServiceRegistration
    {
        public ProfileDocumentViewsCoordinator(
            IProvideJobProfiles jobProfiles,
            IProvideCurrentOpportunities opportunities,
            IProvideSharedContent sharedContent,
            IMapper mapper,
            IFeedbackLinks feedbackLinks,
            ICreateHttpResponseMessages response)
        {
            It.IsNull(jobProfiles)
                .AsGuard<ArgumentNullException>(nameof(jobProfiles));
            It.IsNull(opportunities)
                .AsGuard<ArgumentNullException>(nameof(opportunities));
            It.IsNull(mapper)
                .AsGuard<ArgumentNullException>(nameof(mapper));
            It.IsNull(feedbackLinks)
                .AsGuard<ArgumentNullException>(nameof(feedbackLinks));
            It.IsNull(response)
                .AsGuard<ArgumentNullException>(nameof(response));

            Profiles = jobProfiles;
            Opportunities = opportunities;
            SharedContent = sharedContent;
            Mapper = mapper;
            FeedbackLinks = feedbackLinks;
            Response = response;
        }

        internal ICreateHttpResponseMessages Response { get; }

        internal IProvideJobProfiles Profiles { get; }

        internal IProvideCurrentOpportunities Opportunities { get; }

        internal IProvideSharedContent SharedContent { get; }

        internal IMapper Mapper { get; }

        internal IFeedbackLinks FeedbackLinks { get; }

        public async Task<HttpResponseMessage> GetSummaryDocuments()
        {
            var viewModel = new IndexViewModel();

            var summaries = await Profiles.GetAllItems();

            viewModel.OccupationSummaries = summaries
                .OrderBy(o => o.PageLocation)
                .Select(x => Mapper.Map<OccupationSummaryViewModel>(x))
                .AsSafeReadOnlyList();

            return await Response.Create(HttpStatusCode.OK, viewModel);
        }

        public async Task<HttpResponseMessage> GetDocumentFor(string occupationName, string address)
        {
            var details = await GetDetailsFor<DocumentViewModel>(occupationName);

            details.Model.Body = await AddSupplementalsTo(details.Model.Body);
            details.Model.Body = await AddOpportunitiesTo(details.Model.Body, details.Occupation.CanonicalName);

            details.Model.HeroBanner.Breadcrumb = BuildBreadcrumb(details.Occupation, address);

            return await Response.Create(HttpStatusCode.OK, details.Model);
        }

        public async Task<HttpResponseMessage> GetHeadFor(string occupationName, string address)
        {
            var details = await GetDetailsFor<HeadViewModel>(occupationName);

            details.Model.CanonicalUrl = $"{address}/{details.Occupation.CanonicalName}";

            return await Response.Create(HttpStatusCode.OK, details.Model);
        }

        public async Task<HttpResponseMessage> GetHeroBannerFor(string occupationName, string address)
        {
            var details = await GetDetailsFor<HeroViewModel>(occupationName);
            details.Model.Breadcrumb = BuildBreadcrumb(details.Occupation, address);

            return await Response.Create(HttpStatusCode.OK, details.Model);
        }

        public async Task<HttpResponseMessage> GetBodyFor(Guid occupationID)
        {
            var jobProfile = await Profiles.GetItemBy(occupationID);

            It.IsNull(jobProfile)
                .AsGuard<ResourceNotFoundException>($"Profile details not found for: {occupationID}");

            var model = Mapper.Map<BodyViewModel>(jobProfile);

            model = await AddSupplementalsTo(model);
            model = await AddOpportunitiesTo(model, jobProfile.CanonicalName);

            return await Response.Create(HttpStatusCode.OK, model);
        }

        internal async Task<(TModel Model, JobProfileCached Occupation)> GetDetailsFor<TModel>(string occupationName)
            where TModel : class
        {
            var jobProfile = await Profiles.GetItemBy(occupationName);

            It.IsNull(jobProfile)
                .AsGuard<ResourceNotFoundException>($"Profile details not found for: {occupationName}");

            return (Mapper.Map<TModel>(jobProfile), jobProfile);
        }

        internal async Task<BodyViewModel> AddSupplementalsTo(BodyViewModel candidate)
        {
            var sharedContent = await SharedContent.GetAllItems();

            candidate.SkillsAssessment = GetContent(sharedContent, "skills-assessment-2");
            candidate.SpeakToAnAdvisor = GetContent(sharedContent, "speak-to-an-adviser-2");
            candidate.NotWhatYoureLookingFor = GetContent(sharedContent, "not-what-youre-looking-for");

            candidate.SmartSurveyJP = FeedbackLinks.SmartSurveyJP;

            return candidate;
        }

        internal async Task<BodyViewModel> AddOpportunitiesTo(BodyViewModel candidate, string occupationName)
        {
            var currentOpportunities = await Opportunities.GetItemBy(occupationName);
            candidate.CurrentOpportunities = currentOpportunities;

            return candidate;
        }

        internal Breadcrumb BuildBreadcrumb(JobProfileCached jobProfile, string address) =>
            new Breadcrumb(
                new List<BreadcrumbItem>
                {
                    new BreadcrumbItem("/", "Home"),
                    new BreadcrumbItem($"/{address}", "Job Profiles"),
                    new BreadcrumbItem($"/{address}/{jobProfile.CanonicalName}", jobProfile.Title),
                });

        internal string GetContent(IReadOnlyCollection<StaticItemCached> items, string candidate) =>
            items.FirstOrDefault(x => x.CanonicalName == candidate)?.Content;
    }
}