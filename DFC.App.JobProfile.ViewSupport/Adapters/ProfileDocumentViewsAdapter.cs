using DFC.App.JobProfile.ViewSupport.Coordindators;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using DFC.App.Services.Common.Adapters;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Adapters
{
    internal sealed class ProfileDocumentViewsAdapter :
        IAdaptProfileDocumentViews,
        IRequireServiceRegistration
    {
        public ProfileDocumentViewsAdapter(
            ICoordinateProfileDocumentViews coordinator,
            IAdaptActionResultOperations adapter)
        {
            It.IsNull(coordinator)
                .AsGuard<ArgumentNullException>(nameof(coordinator));
            It.IsNull(adapter)
                .AsGuard<ArgumentNullException>(nameof(adapter));

            Coordinator = coordinator;
            Adapter = adapter;
        }

        internal ICoordinateProfileDocumentViews Coordinator { get; }

        internal IAdaptActionResultOperations Adapter { get; }

        Task<IActionResult> IAdaptProfileDocumentViews.GetSummaryView(
            Func<IndexViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetSummary(), contentResult, $"Getting the document summaries");

        Task<IActionResult> IAdaptProfileDocumentViews.GetDocumentViewFor(
            string occupationName,
            string address,
            Func<DocumentViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetDocument(occupationName, address), contentResult, $"Getting the document for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetHeadViewFor(
            string occupationName,
            string address,
            Func<HeadViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetHead(occupationName, address), contentResult, $"Getting the head for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetHeroBannerViewFor(
            string occupationName,
            string address,
            Func<HeroViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetHeroBanner(occupationName, address), contentResult, $"Getting the hero banner for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetBodyViewFor(
            Guid occupationID,
            Func<BodyViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetBody(occupationID), contentResult, $"Getting the document body for '{occupationID}'");

        internal Task<HttpResponseMessage> ProcessGetSummary() =>
            Coordinator.GetSummaryDocuments();

        internal Task<HttpResponseMessage> ProcessGetDocument(string occupationName, string address) =>
            Coordinator.GetDocumentFor(occupationName, address);

        internal Task<HttpResponseMessage> ProcessGetHead(string occupationName, string address) =>
            Coordinator.GetHeadFor(occupationName, address);

        internal Task<HttpResponseMessage> ProcessGetHeroBanner(string occupationName, string address) =>
            Coordinator.GetHeroBannerFor(occupationName, address);

        internal Task<HttpResponseMessage> ProcessGetBody(Guid occupationID) =>
            Coordinator.GetBodyFor(occupationID);
    }
}
