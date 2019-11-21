using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.JobProfile.Data.Models
{
    public class HealthCheckItems
    {
        public Uri Source { get; set; }

        public IList<HealthCheckItem> HealthItems { get; set; }
    }
}
