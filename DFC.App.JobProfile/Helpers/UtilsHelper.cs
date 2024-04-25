using System.Collections.Generic;
using System.Linq;

namespace DFC.App.JobProfile.Helpers
{
    public static class UtilsHelper
    {
        public static bool IsAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}
