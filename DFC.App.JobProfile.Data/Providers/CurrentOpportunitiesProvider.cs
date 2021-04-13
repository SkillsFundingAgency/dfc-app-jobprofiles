using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Providers
{
    [ExcludeFromCodeCoverage]
    internal sealed class CurrentOpportunitiesProvider :
        ContentPageProvider<CurrentOpportunities>,
        IProvideCurrentOpportunities
    {
        public CurrentOpportunitiesProvider(
            IContentPageService<CurrentOpportunities> pageService)
            : base(pageService)
        {
        }
    }
}