using Newtonsoft.Json;
using System;

namespace DFC.App.JobProfile.Data.Contracts
{
    public interface IApiDataModel
    {
        [JsonProperty("Uri")]
        Uri? Url { get; set; }
    }
}
