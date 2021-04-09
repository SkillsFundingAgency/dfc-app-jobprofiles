using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DFC.App.Services.Common.Helpers
{
    /// <summary>
    /// A static helper class to increase code readability.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class It
    {
        /// <summary>
        /// Is null...
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>True if null.</returns>
        public static bool IsNull<T>(T item)
            where T : class =>
                item == null;

        /// <summary>
        /// Has...
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>True if not null.</returns>
        public static bool Has<T>(T item)
            where T : class =>
                !IsNull(item);

        /// <summary>
        /// Has values...
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>True if not null and has values.</returns>
        public static bool HasValues<T>(IEnumerable<T> values) =>
            Has(values) && values.Any();

        /// <summary>
        /// Is empty.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if null or whitespace.</returns>
        public static bool IsEmpty(string item) =>
            string.IsNullOrWhiteSpace(item);

        /// <summary>
        /// Is empty...
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>True if null or empty.</returns>
        public static bool IsEmpty<T>(IEnumerable<T> values) =>
            IsNull(values) || !values.Any();

        /// <summary>
        /// Determines whether the specified candidate is inside the given range.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>true or false.</returns>
        public static bool IsBetween(int candidate, int min, int max, bool includeBoundaries = true) =>
            includeBoundaries
                ? candidate >= min && candidate <= max
                : candidate > min && candidate < max;

        /// <summary>
        /// Determines whether the specified candidate is not inside the given range.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="includeBoundaries">if set to <c>true</c> [include boundaries].</param>
        /// <returns>true or false.</returns>
        public static bool IsNotBetween(int candidate, int min, int max, bool includeBoundaries = true) =>
            !IsBetween(candidate, min, max, includeBoundaries);
    }
}