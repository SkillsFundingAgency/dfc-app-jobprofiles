using DFC.App.JobProfile.ViewSupport.Coordindators;
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

        Task<IActionResult> IAdaptProfileDocumentViews.GetSummaryDocuments<TModel>(
            Func<TModel, IActionResult> contentResult) =>
                Adapter.Run(() => GetSummaryDocuments(), contentResult, $"Getting the document summaries");

        Task<IActionResult> IAdaptProfileDocumentViews.GetDocumentViewFor<TModel>(
            string occupationName,
            Func<TModel, IActionResult> contentResult) =>
                Adapter.Run(() => ProcessGetRequest(occupationName), contentResult, $"Getting the document view for '{occupationName}'");

        internal async Task<HttpResponseMessage> ProcessGetRequest(string occupationName) =>
            await Coordinator.GetDocumentFor(occupationName);

        internal async Task<HttpResponseMessage> GetSummaryDocuments() =>
            await Coordinator.GetSummaryDocuments();
    }
}
