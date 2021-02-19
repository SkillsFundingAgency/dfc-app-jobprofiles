using System;
using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.Data.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<TItem> Flatten<TItem, TRecursion>(this IEnumerable<TItem> source, Func<TItem, TRecursion> recursion)
          where TRecursion : IEnumerable<TItem>
        {
            var flattened = source.ToList();

            var children = source.Select(recursion);

            if (children != null)
            {
                foreach (var child in children)
                {
                    flattened.AddRange(child.Flatten(recursion));
                }
            }

            return flattened;
        }
    }
}
