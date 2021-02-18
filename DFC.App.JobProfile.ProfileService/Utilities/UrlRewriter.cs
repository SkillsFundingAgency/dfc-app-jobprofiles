using System;

namespace DFC.App.JobProfile.ProfileService.Utilities
{
    public static class UrlRewriter
    {
        public static string Rewrite(string content, Uri fromUri, Uri toUri)
        {
            if (fromUri != null && toUri != null && content != null)
            {
                var attributeNames = new string[] { "href", "action" };
                var quoteChars = new char[] { '"', '\'' };

                foreach (var attributeName in attributeNames)
                {
                    foreach (var quoteChar in quoteChars)
                    {
                        var fromUrlPrefixes = new string[]
                        {
                            $"{attributeName}={quoteChar}/",
                            $"{attributeName}={quoteChar}{fromUri.ToString()}",
                        };
                        var toUrlPrefix = $"{attributeName}={quoteChar}{toUri.ToString()}";

                        foreach (var fromUrlPrefix in fromUrlPrefixes)
                        {
                            if (content.Contains(fromUrlPrefix, StringComparison.InvariantCultureIgnoreCase))
                            {
                                content = content.Replace(fromUrlPrefix, toUrlPrefix, StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }
                }
            }

            return content;
        }
    }
}