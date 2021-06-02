using DFC.App.JobProfile.ViewSupport.Coordindators;
using DFC.App.JobProfile.ViewSupport.ViewModels;
using DFC.App.Services.Common.Adapters;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Adapters
{
    [ExcludeFromCodeCoverage]
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
                Adapter.Run(() => Coordinator.GetSummaryDocuments(), contentResult, $"Getting the document summaries");

        Task<IActionResult> IAdaptProfileDocumentViews.GetHeadViewFor(
            string occupationName,
            string address,
            Func<HeadViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetHeadFor(occupationName, address), contentResult, $"Getting the head for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetHeroBannerViewFor(
            string occupationName,
            string address,
            Func<HeroViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetHeroBannerFor(occupationName, address), contentResult, $"Getting the hero banner for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetBodyViewFor(
            Guid occupationID,
            Func<BodyViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetBodyFor(occupationID), contentResult, $"Getting the document body for '{occupationID}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetBodyViewFor(
            string occupationName,
            Func<BodyViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetBodyFor(occupationName), contentResult, $"Getting the document body for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetDocumentOverviewFor(
            string occupationName,
            string address,
            Func<HeroViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetHeroBannerFor(occupationName, address), contentResult, $"Getting the document overview for '{occupationName}'");

        Task<IActionResult> IAdaptProfileDocumentViews.GetDocumentViewFor(
            string occupationName,
            string address,
            Func<DocumentViewModel, IActionResult> contentResult) =>
                Adapter.Run(() => Coordinator.GetDocumentFor(occupationName, address), contentResult, $"Getting the document for '{occupationName}'");
    }
}
