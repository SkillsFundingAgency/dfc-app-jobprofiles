using DFC.Content.Pkg.Netcore.Data.Models;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IPagesApiDataModel
    {
        ContentLinksModel? ContentLinks { get; set; }

        //IList<ApiContentItemModel> ContentItems { get; set; }
    }
}
