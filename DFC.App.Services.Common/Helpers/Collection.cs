using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Helpers
{
    /// <summary>
    /// A collection helper.
    /// </summary>
    public static class Collection
    {
        /// <summary>
        /// Empty (collection).
        /// </summary>
        /// <typeparam name="T">of type <typeparamref name="T"/>.</typeparam>
        /// <returns>A safe empty collection.</returns>
        public static ICollection<T> Empty<T>() =>
            new List<T>();

        /// <summary>
        /// Empty and readonly (collection).
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <returns>A safe empty readonly collection.</returns>
        public static IReadOnlyCollection<T> EmptyAndReadOnly<T>() =>
            Empty<T>().AsSafeReadOnlyList();

        /// <summary>
        /// For each, to safe list and conducts the action.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>();

            var items = collection.SafeList();
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Safe any...
        /// Null object pattern safety.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>True for any instance in the collection where the predicate returned true.</returns>
        public static bool SafeAny<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            It.IsNull(predicate)
                .AsGuard<ArgumentNullException>();

            var items = collection.SafeList();
            return items.Any(predicate);
        }

        /// <summary>
        /// As a safe readonly list.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A readonly safe collection.</returns>
        public static IReadOnlyCollection<T> AsSafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return list.SafeReadOnlyList();
        }

        /// <summary>
        /// As a safe readonly list, task / async compatible.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A readonly safe collection.</returns>
        public static Task<IReadOnlyCollection<T>> AsSafeReadOnlyList<T>(this Task<IEnumerable<T>> list)
        {
            return list.SafeReadOnlyList();
        }

        /// <summary>
        /// Safe read only list.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A safe readonly collection.</returns>
        private static IReadOnlyCollection<T> SafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return new ReadOnlyCollection<T>(list.SafeList());
        }

        /// <summary>
        /// Safe read only list, task / async compatible.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A safe readonly collection.</returns>
        private static async Task<IReadOnlyCollection<T>> SafeReadOnlyList<T>(this Task<IEnumerable<T>> list)
        {
            return new ReadOnlyCollection<T>(await list.SafeList());
        }

        /// <summary>
        /// Safe list, the private implementation of null coalescing.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A safe list.</returns>
        private static List<T> SafeList<T>(this IEnumerable<T> list)
        {
            return (list ?? new List<T>()).ToList();
        }

        /// <summary>
        /// Safe list, the private implementation of null coalescing, task / async compatible.
        /// </summary>
        /// <typeparam name="T">Of type <typeparamref name="T"/>.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A safe list.</returns>
        private static async Task<List<T>> SafeList<T>(this Task<IEnumerable<T>> list)
        {
            return (await list ?? new List<T>()).ToList();
        }
    }
}