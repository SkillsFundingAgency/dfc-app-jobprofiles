﻿using DFC.App.JobProfile.Data.Contracts;
using DFC.App.JobProfile.Data.Enums;
using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models.Segments.HowToBecomeModels
{
    public class HowToBecomeSegmentDataModel : ISegmentDataModel
    {
        public const string SegmentName = "HowToBecome";

        public DateTime LastReviewed { get; set; }

        public string Title { get; set; }

        public TitlePrefix TitlePrefix { get; set; }

        public string EntryRouteSummary { get; set; }

        public EntryRoutes EntryRoutes { get; set; }

        public MoreInformation MoreInformation { get; set; }

        public IEnumerable<GenericListContent> Registrations { get; set; }
    }
}
