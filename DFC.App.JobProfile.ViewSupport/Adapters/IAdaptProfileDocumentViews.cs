using DFC.App.JobProfile.ViewSupport.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.JobProfile.ViewSupport.Adapters
{
    public interface IAdaptProfileDocumentViews
    {
        Task<IActionResult> GetSummaryView(
            Func<IndexViewModel, IActionResult> contentResult);

        Task<IActionResult> GetHeadViewFor(
            string occupationName,
            string address,
            Func<HeadViewModel, IActionResult> contentResult);

        Task<IActionResult> GetHeroBannerViewFor(
            string occupationName,
            string address,
            Func<HeroViewModel, IActionResult> contentResult);

        Task<IActionResult> GetBodyViewFor(
            Guid occupationID,
            Func<BodyViewModel, IActionResult> contentResult);

        Task<IActionResult> GetDocumentOverviewFor(
            string occupationName,
            string address,
            Func<HeroViewModel, IActionResult> contentResult);

        Task<IActionResult> GetDocumentViewFor(
            string occupationName,
            string address,
            Func<DocumentViewModel, IActionResult> contentResult);
    }
}