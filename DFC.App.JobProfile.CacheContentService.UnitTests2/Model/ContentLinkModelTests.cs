using DFC.App.JobProfile.Data.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Xunit;

namespace DFC.App.JobProfile.CacheContentService.UnitTests.Model
{
    public class ContentLinkModelTests
    {
        [Fact]
        public void WhenContentLinksIsNotNullThenReturnLinks()
        {
            var jsonData = File.ReadAllText(Directory.GetCurrentDirectory() + "/Model/LinksData.json");
            var model = new ContentLinksModel(JObject.Parse(jsonData));
            var links = model.ContentLinks;
            Assert.Equal(4, links.SelectMany(x => x.Value).Count());
        }

        [Fact]
        public void WhenContentLinksIsNullThenReturnEmptyList()
        {
            var jsonData = "{}";
            var model = new ContentLinksModel(JObject.Parse(jsonData));
            var links = model.ContentLinks;
            Assert.False(links.Any());
        }
    }
}
