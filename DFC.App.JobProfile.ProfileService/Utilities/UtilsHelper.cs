using DFC.App.JobProfile.Data.Models.Overview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DFC.App.JobProfile.ProfileService.Utilities
{
    public static class UtilsHelper
    {
        public static bool IsAny<T>(this List<T> data)
        {
            return data != null && data.Any();
        }

        public static string AddPrefix(this string jobTitle)
        {
            if (string.IsNullOrEmpty(jobTitle))
            {
                return "a";
            }
            else if ("AEIOUaeiou".IndexOf(jobTitle[0]) != -1)
            {
                return "an";
            }
            else
            {
                return "a";
            }
        }

        public static string ConvertCourseKeywordsString(this string input)
        {
            // Regular expression pattern to match substrings within single quotes
            string pattern = @"'([^']*)'";

            // Find all matches of substrings within single quotes, extract substrings from matches, join by a comma and convert to a string
            var result = string.Join(",", Regex.Matches(input, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(1))
                .OfType<Match>()
                .Select(m => m.Groups[1].Value));

            return result;
        }
    }
}
