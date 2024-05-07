using DFC.App.JobProfile.Data.Contracts;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.JobProfile.Data.Models
{
    public class CurrentOpportunitiesSegmentModel : IDataModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid DocumentId { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        public string Etag { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        public string PartitionKey => SocLevelTwo;

        [Required]
        public string SocLevelTwo { get; set; }

        [Required]
        public long SequenceNumber { get; set; }

        public CurrentOpportunitiesSegmentDataModel Data { get; set; }
    }
}
