using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Services.Common.Helpers
{
    /// <summary>
    /// A collection of routine grafts for strings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StringHelpers
    {
        public static bool ComparesWith(this string source, string candidate)
        {
            return string.Compare(source, candidate, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool ComparesWith(this string source, int candidate)
        {
            return source.ComparesWith($"{candidate}");
        }
    }
}