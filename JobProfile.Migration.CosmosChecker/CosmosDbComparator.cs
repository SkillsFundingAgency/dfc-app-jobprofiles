using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using DFC.App.JobProfile.Data;
using DFC.App.JobProfile.Data.Models;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

using static JobProfile.Migration.CosmosChecker.Helpers;

namespace JobProfile.Migration.CosmosChecker
{
    public class CosmosDbComparator
    {
        private const string errorFileName = "CosmosFailureLog.txt";
        static RetryOptions _retryOptions = new RetryOptions { MaxRetryAttemptsOnThrottledRequests = 20, MaxRetryWaitTimeInSeconds = 60 };

        private static readonly IDocumentClient _documentClientOld = new DocumentClient(
            new Uri("https://localhost:8081"),
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new ConnectionPolicy
            {
                RetryOptions = _retryOptions
            });

        private static readonly IDocumentClient _documentClientNew = new DocumentClient(
            new Uri("https://localhost:8081"),
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new ConnectionPolicy
            {
                RetryOptions = _retryOptions
            });

        private Uri DocumentCollectionUriOld => UriFactory.CreateDocumentCollectionUri("jp-prod", "jp");
        private Uri DocumentCollectionUriNew => UriFactory.CreateDocumentCollectionUri("dfc-app-jobprofiles", "jobProfiles");

        public async Task CompareCosmosDb()
        {
            Console.WriteLine("Starting comparison...");
            int total = 0, matching = 0;
            var existingProfiles = await GetAllAsync(_documentClientOld, DocumentCollectionUriOld);

            var newProfiles = await GetAllAsync(_documentClientNew, DocumentCollectionUriNew);
            foreach (var profile in existingProfiles)
            {
                total++;
                var newProfile = newProfiles.FirstOrDefault(p => p.CanonicalName.Equals(profile.CanonicalName));
                (bool doesMatch, string property) = DoesMatch(newProfile, profile);
                if (doesMatch)
                {
                    matching++;
                    Console.WriteLine($"{total}: SUCCESS: Cosmos document matched for profile: {profile.CanonicalName}");
                }
                else
                {
                    await File.AppendAllTextAsync(errorFileName, $"FAILED: Cosmos document does not match for profile: {profile.CanonicalName} \r\n {property}.");
                    await File.AppendAllLinesAsync(errorFileName, new[] { "", "*********************************************", "" });
                    //if (property.Contains("abillity"))
                    //    await File.AppendAllLinesAsync("AAAA.txt", new[] { profile.MetaTags.Title });
                    Console.WriteLine($"{total}: FAILED: Cosmos document does not match for profile: {profile.CanonicalName} - {property}.");

                }

            }

            Console.WriteLine($"Finished comparison. Total profiles: {total}. Matching profiles: {matching}");
        }

        public (bool, string) DoesMatch(JobProfileModel newProfile, JobProfileModel oldProfile)
        {
            if (newProfile is null)
                return (false, "newProfile is null");

            if (oldProfile is null)
                return (false, "oldProfile is null");

            if (AreEqualOrNull(newProfile.JobProfileId.ToString(), oldProfile.JobProfileId.ToString()) is false)
                return (false, Difference("JobProfileId", oldProfile.JobProfileId.ToString(), newProfile.JobProfileId.ToString()));

            if (AreEqualOrNull(newProfile.BreadcrumbTitle, oldProfile.BreadcrumbTitle) is false)
                return (false, Difference("BreadcrumbTitle", oldProfile.BreadcrumbTitle, newProfile.BreadcrumbTitle));

            if (AreEqualOrNull(newProfile.MetaTags.Title, oldProfile.MetaTags.Title) is false)
                return (false, Difference("MetaTags.Title", oldProfile.MetaTags.Title, newProfile.MetaTags.Title));

            if (AreEqualOrNull(newProfile.MetaTags.Description, oldProfile.MetaTags.Description) is false)
                return (false, Difference("MetaTags.Description", oldProfile.MetaTags.Description, newProfile.MetaTags.Description));

            foreach (var segment in oldProfile.Segments)
            {
                // Not checking current opportunities as the courses and apprenticeship vacancies will always change.
                if (segment.Segment is JobProfileSegment.CurrentOpportunities)
                    return (true, string.Empty);

                var newSeg = newProfile.Segments.FirstOrDefault(s => s.Segment.Equals(segment.Segment));

                if (newSeg is null)
                    return (false, segment.Segment.ToString());

                switch (segment.Segment)
                {
                    case JobProfileSegment.RelatedCareers:
                        if (AreEqualOrNull(segment.Markup.Value.CleanUrl(), newSeg.Markup.Value.CleanUrl()) is false)
                            return (false, Difference($"{segment.Segment}.Markup", segment.Markup.Value, newSeg.Markup.Value));
                        break;
                    case JobProfileSegment.Overview:
                    //break;
                    case JobProfileSegment.HowToBecome:
                    //break;
                    case JobProfileSegment.WhatItTakes:
                    //break;
                    case JobProfileSegment.WhatYouWillDo:
                    //break;
                    case JobProfileSegment.CareerPathsAndProgression:
                    //break;
                    case JobProfileSegment.CurrentOpportunities:
                    //break;
                    default:
                        if (AreEqualOrNull(segment.Markup.Value.Clean(), newSeg.Markup.Value.Clean()) is false)
                            return (false, Difference($"{segment.Segment}.Markup", segment.Markup.Value, newSeg.Markup.Value));
                        break;
                }

                switch (segment.Segment)
                {
                    case JobProfileSegment.Overview:
                        if (CompareJson<OverviewJson>(segment.JsonV1, newSeg.JsonV1) is false)
                            return (false, Difference("Overview.JsonV1", segment.JsonV1, newSeg.JsonV1));
                        break;
                    case JobProfileSegment.HowToBecome:
                        if (CompareJson<HowToBecomeJson>(segment.JsonV1, newSeg.JsonV1) is false)
                            return (false, Difference("HowToBecomeJson.JsonV1", segment.JsonV1, newSeg.JsonV1));
                        break;
                    case JobProfileSegment.WhatItTakes:
                        if (CompareJson<WhatItTakesJson>(segment.JsonV1, newSeg.JsonV1) is false)
                            return (false, Difference("WhatItTakesJson.JsonV1", segment.JsonV1, newSeg.JsonV1));
                        break;
                    case JobProfileSegment.WhatYouWillDo:
                    //break;
                    case JobProfileSegment.CareerPathsAndProgression:
                    //break;
                    case JobProfileSegment.CurrentOpportunities:
                    //break;
                    case JobProfileSegment.RelatedCareers:
                        if (AreEqualOrNull(segment.JsonV1.CleanUrl(), newSeg.JsonV1.CleanUrl()) is false)
                            return (false, Difference($"{segment.Segment}.JsonV1", segment.JsonV1, newSeg.JsonV1));
                        break;
                    default:
                        if (AreEqualOrNull(segment.JsonV1.Clean(), newSeg.JsonV1.Clean()) is false)
                            return (false, Difference($"{segment.Segment}.JsonV1", segment.JsonV1, newSeg.JsonV1));
                        break;
                }
            }

            return (true, string.Empty);
        }

        private bool CompareJson<T>(string jsonOld, string jsonNew)
        {
            if (jsonOld is null && jsonNew is null) return true;
            if ((jsonOld is null && jsonNew != null) || (jsonNew is null && jsonOld != null)) return false;

            var oldItem = JsonSerializer.Deserialize<T>(jsonOld);
            var newItem = JsonSerializer.Deserialize<T>(jsonNew);

            if (newItem is HowToBecomeJson htb)
            {
                htb.entryRoutes.university.entryRequirementPreface = SetEmptyNull(htb.entryRoutes.university.entryRequirementPreface);
                htb.entryRoutes.apprenticeship.entryRequirementPreface = SetEmptyNull(htb.entryRoutes.apprenticeship.entryRequirementPreface);
                htb.entryRoutes.college.entryRequirementPreface = SetEmptyNull(htb.entryRoutes.college.entryRequirementPreface);
            }

            if (newItem is OverviewJson ovNew)
                ovNew.alternativeTitle = SetEmptyNull(ovNew.alternativeTitle);

            if (oldItem is OverviewJson ovOld)
                ovOld.alternativeTitle = SetEmptyNull(ovOld.alternativeTitle);

            return AreEqualOrNull(JsonSerializer.Serialize(oldItem).Clean(), JsonSerializer.Serialize(newItem).Clean());

        }
        private static string SetEmptyNull(string preface)
        {
            return preface?.Equals(string.Empty) is true ? null : preface;
        }

        public async Task<IEnumerable<JobProfileModel>> GetAllAsync(IDocumentClient client, Uri documentCollectionUri)
        {
            var query = client.CreateDocumentQuery<JobProfileModel>(documentCollectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                                      .AsDocumentQuery();

            var models = new List<JobProfileModel>();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<JobProfileModel>();
                models.AddRange(result);
            }

            return models;
        }


        public class OverviewJson
        {
            public string alternativeTitle { get; set; }
            public object maximumHours { get; set; }
            public object minimumHours { get; set; }
            public string oNetOccupationalCode { get; set; }
            public string overview { get; set; }
            public string salaryExperienced { get; set; }
            public string salaryStarter { get; set; }
            public string soc { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string workingHoursDetails { get; set; }
            public string workingPattern { get; set; }
            public string workingPatternDetails { get; set; }
        }


        public class HowToBecomeJson
        {
            public Entryroutes entryRoutes { get; set; }
            public string[] entryRouteSummary { get; set; } = Array.Empty<string>();
            public Moreinformation moreInformation { get; set; }
        }

        public class Entryroutes
        {
            public Information apprenticeship { get; set; }
            public Information college { get; set; }
            public string[] directApplication { get; set; } = Array.Empty<string>();
            public string[] otherRoutes { get; set; } = Array.Empty<string>();
            public Information university { get; set; }
            public object[] volunteering { get; set; } = Array.Empty<object>();
            public object[] work { get; set; } = Array.Empty<object>();
        }

        public class Information
        {
            public string[] additionalInformation { get; set; } = Array.Empty<string>();
            public string entryRequirementPreface { get; set; } = string.Empty;
            public string[] entryRequirements { get; set; } = Array.Empty<string>();
            public string[] furtherInformation { get; set; } = Array.Empty<string>();
            public string[] relevantSubjects { get; set; } = Array.Empty<string>();
        }

        public class Moreinformation
        {
            public string[] careerTips { get; set; } = Array.Empty<string>();
            public string[] furtherInformation { get; set; } = Array.Empty<string>();
            public object[] professionalAndIndustryBodies { get; set; } = Array.Empty<object>();
            public object[] registrations { get; set; } = Array.Empty<object>();
        }


        public class WhatItTakesJson
        {
            public string digitalSkillsLevel { get; set; }
            public Skill[] skills { get; set; }
            public Restrictionsandrequirements restrictionsAndRequirements { get; set; }
        }

        public class Restrictionsandrequirements
        {
            public object[] relatedRestrictions { get; set; }
            public object[] otherRequirements { get; set; }
        }

        public class Skill
        {
            public string description { get; set; }
            public string oNetAttributeType { get; set; }
            public string oNetElementId { get; set; }
        }

    }
}
