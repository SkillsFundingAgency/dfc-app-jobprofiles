using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.WhatItTakesModels
{
    public class WhatItTakesSegmentModel : BaseSegmentModel
    {
        public const string SegmentName = "WhatItTakes";

        public WhatItTakesSegmentDataModel Data { get; set; }
    }
}
