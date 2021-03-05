using DFC.App.JobProfile.Data.Models;
using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.JobProfile.Data.Providers
{
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