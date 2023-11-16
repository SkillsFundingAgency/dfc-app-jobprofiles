using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.Helpers
{
    public static class TextHelpers
    {
        private static readonly string[] EndOfLineSequences = { "\r\n", "\r", "\n" };

        /// <summary>
        /// Split input text into an enumerable collection of lines.
        /// </summary>
        /// <param name="text">Plain text with line breaks.</param>
        /// <returns>
        /// An array of zero-or-more lines from the input text.
        /// </returns>
        public static IEnumerable<string> SplitTextIntoLines(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            return text
                .Split(EndOfLineSequences, StringSplitOptions.None)
                .Select(line => line.Trim())
                .Where(line => line != string.Empty);
        }
    }
}
