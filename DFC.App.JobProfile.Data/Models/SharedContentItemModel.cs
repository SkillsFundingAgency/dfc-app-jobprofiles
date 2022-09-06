using DFC.App.JobProfile.Data.Contracts;
using DFC.Compui.Cosmos.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.JobProfile.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class SharedContentItemModel : DocumentModel, ICachedModel
    {
        public const string DefaultPartitionKey = "shared-content";

        public override string? PartitionKey { get; set; } = DefaultPartitionKey;

        [Required]
        public string? Title { get; set; }

        [Required]
        public Uri? Url { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime LastReviewed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;
    }
}