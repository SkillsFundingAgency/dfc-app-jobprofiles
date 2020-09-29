using DFC.Content.Pkg.Netcore.Data.Models;

namespace DFC.App.JobProfile.Data.Models
{
    public class JobProfileItemModel : ApiSummaryItemModel
    {
        public string CanonicalName
        {
            get => Title;

            set => Title = value;
        }
    }
}
