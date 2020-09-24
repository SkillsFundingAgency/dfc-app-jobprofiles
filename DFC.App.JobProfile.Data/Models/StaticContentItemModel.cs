using DFC.App.JobProfile.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class StaticContentItemModel : Compui.Cosmos.Models.ContentPageModel, IApiDataModel, IDataModel
    {
        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string skos_prefLabel { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri Url { get; set; }

        //[JsonProperty(PropertyName = "id")]
        //public string Id { get; set; }

        [JsonProperty(PropertyName = "htmlbody_Html")]
        public string Html_Content { get; set; }

        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public override string PageLocation { get; set; } = "/shared-content";

        Guid IDataModel.DocumentId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IDataModel.Etag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        string IDataModel.PartitionKey => throw new NotImplementedException();
    }
}
