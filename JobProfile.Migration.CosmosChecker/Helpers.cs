using System;
using System.Collections.Generic;
using System.Linq;

namespace JobProfile.Migration.CosmosChecker
{
    public static class Helpers
    {
        public static bool AreEqualOrNull(double field1, double field2) =>
            field1.Equals(field2);

        public static bool AreEqualOrNull(string field1, string field2) =>
            (field1 is null && field2 is null) || field1?.Equals(field2) is true;

        public static bool AreEqualOrNull(IEnumerable<string> @new, IEnumerable<string> old) =>
            (@new is null && old is null) ||
                (@new.Where(n => !string.IsNullOrWhiteSpace(n)).Except(old).Any() is false && old.Where(n => !string.IsNullOrWhiteSpace(n)).Except(@new).Any() is false);

        public static string Difference(string prop, string oldValue, string newValue) =>
                    $"{prop}{Environment.NewLine}OLD: {oldValue}{Environment.NewLine}NEW: {newValue}";

        public static string Difference(string prop, IEnumerable<string> oldValue, IEnumerable<string> newValue) =>
            Difference(prop, string.Join(", ", oldValue), string.Join(", ", newValue));
        public static string Clean(this string val)
        {
            return val?
                // temporary
                //.Replace("hand held", "hand-held")
                //.Replace("abillity", "ability")
                // temp - end
                .Replace(" ", string.Empty)
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("<br/>", "<br>")
                .Replace("&ndash;", "-")
                .Replace("&rsquo;", "’")
                .Replace("&lsquo;", "‘");
        }


        public static string CleanUrl(this string url) =>
            url?
            .Replace("-needs-(sen)-", "-needs-sen-")
            .Replace("production-worker-(manufacturing)", "production-worker-manufacturing")
            .Replace("barrister's-clerk", "barristers-clerk")
            .Replace("immigration-adviser-(non-government)", "immigration-adviser-non-government")
            .Replace("children's-nurse", "childrens-nurse")
            .Replace("builders'-merchant", "builders-merchant");
    }
}
