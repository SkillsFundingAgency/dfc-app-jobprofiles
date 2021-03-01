// TODO: breadcumb trail...
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
using AutoMapper;
using DFC.App.JobProfile.Data.Models;
using DFC.App.JobProfile.Data.Providers;
using DFC.App.JobProfile.ViewSupport.Configuration;
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

            var summaries = await Profiles.GetAllItems().ConfigureAwait(false);

            viewModel.OccupationSummaries = summaries
                .OrderBy(o => o.PageLocation)
                .Select(x => Mapper.Map<OccupationSummaryViewModel>(x))
                .AsSafeReadOnlyList();

            return await Response.Create(HttpStatusCode.OK, viewModel);
        }

        public async Task<HttpResponseMessage> GetDocumentFor(string occupationName)
        {
            var jobProfile = await Profiles.GetItemBy(occupationName).ConfigureAwait(false);

            It.IsNull(jobProfile)
                .AsGuard<ResourceNotFoundException>($"Profile details not found for: {occupationName}");
            var viewModel = Mapper.Map<DocumentViewModel>(jobProfile);

            var sharedContent = await SharedContent.GetAllItems().ConfigureAwait(false);
            viewModel.Body.SkillsAssessment = GetContent(sharedContent, "skills-assessment-2");
            viewModel.Body.SpeakToAnAdvisor = GetContent(sharedContent, "speak-to-an-adviser-2");
            viewModel.Body.NotWhatYoureLookingFor = GetContent(sharedContent, "not-what-youre-looking-for");
            viewModel.Body.SmartSurveyJP = FeedbackLinks.SmartSurveyJP;

            var currentOpportunities = await Opportunities.GetItemBy("jobprofile-canonicalname");
            It.IsNull(currentOpportunities)
                .AsGuard<ResourceNotFoundException>($"Profile details not found for: {occupationName}");

            viewModel.Body.CurrentOpportunities = currentOpportunities;

            //viewModel.Breadcrumb = BuildBreadcrumb(jobProfileModel)

            return await Response.Create(HttpStatusCode.OK, viewModel);
        }

        internal string GetContent(IReadOnlyCollection<StaticItemCached> items, string candidate) =>
            items.FirstOrDefault(x => x.CanonicalName == candidate)?.Content;
    }
}