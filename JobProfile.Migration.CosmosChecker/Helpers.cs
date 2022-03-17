using System.Collections.Generic;
using System.Linq;

namespace JobProfile.Migration.CosmosChecker
{
    public static class Helpers
    {
        public static bool AreEqualOrNull(object field1, object field2) =>
            (field1 is null && field2 is null) || field1?.Equals(field2) is true;

        public static bool AreEqualOrNull(IEnumerable<string> @new, IEnumerable<string> old) =>
            (@new is null && old is null) ||
                (@new.Except(old).Any() is false && old.Except(@new).Any() is false);

    }
}
