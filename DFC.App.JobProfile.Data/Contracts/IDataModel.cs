using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IDataModel
    {
        [Required]
        [JsonProperty(PropertyName = "id")]
        Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        string Etag { get; set; }

        [Required]
        string PartitionKey { get; }
    }
}
