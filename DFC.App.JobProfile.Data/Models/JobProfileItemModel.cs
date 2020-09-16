using dfc_content_pkg_netcore.models;

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
