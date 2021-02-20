using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Cosmos.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.Data.Models
{
    public class ContentApiStaticElement :
        ContentPageModel,
        IResourceLocationModel
    {
        [JsonProperty("skos__prefLabel")]
        public string Title { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty(PropertyName = "htmlbody_Html")]
        public string HtmlContent { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        public override string PageLocation { get; set; } = "/shared-content";
    }
}
