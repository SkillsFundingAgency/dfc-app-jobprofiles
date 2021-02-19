using DFC.Content.Pkg.Netcore.Data.Models;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileSummaryModel :
        SummaryItemModel
    {
        public string CanonicalName
        {
            get => Title ??= string.Empty;

            set => Title = value;
        }
    }
}
