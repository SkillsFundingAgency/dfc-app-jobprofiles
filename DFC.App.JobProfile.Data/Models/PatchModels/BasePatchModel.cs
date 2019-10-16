using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DFC.App.JobProfile.Data
{
    public class BasePatchModel
    {
        [Required]
        public Guid JobProfileId { get; set; }

        [Required]
        public string CanonicalName { get; set; }

        [Required]
        public MessageAction MessageAction { get; set; }

        [Required]
        public long SequenceNumber { get; set; }
    }
}
