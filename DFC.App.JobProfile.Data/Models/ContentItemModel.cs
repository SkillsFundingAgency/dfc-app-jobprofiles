﻿using System;
using System.Collections.Generic;

namespace DFC.App.JobProfile.Data.Models
{
    public class ContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string? DisplayText { get; set; }

        public Guid Version { get; set; }

        public string? Content { get; set; }

        public string? Alignment { get; set; }

        public int Ordinal { get; set; }

        public int Size { get; set; }

        public DateTime? LastReviewed { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? Title { get; set; }

        public string? ContentType { get; set; }

        public string? HtmlBody { get; set; }

        public IList<SharedContentItemModel>? ContentItems { get; set; }

    }
}