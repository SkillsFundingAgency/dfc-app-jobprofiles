using DFC.App.Services.Common.Helpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFC.App.Services.Common.Tests.Helpers
{
    [TestClass]
    public class StringHelperTests
    {
        [TestMethod]
        [DataRow("true", "blue", false)]
        [DataRow("trueblue", "trueblue", true)]
        [DataRow("trueBlue", "trueblue", true)]
        [DataRow("trueBlue", "TrueBlue", true)]
        public void ComparesWithStringMeetsExpectation(string source, string candidate, bool expectation)
        {
            // arrange / act / assert
            source.ComparesWith(candidate).Should().Be(expectation);
        }

        [TestMethod]
        [DataRow("true", 12, false)]
        [DataRow("12", 12, true)]
        [DataRow("12a", 12, false)]
        [DataRow("a12", 12, false)]
        public void ComparesWithIntegerMeetsExpectation(string source, int candidate, bool expectation)
        {
            // arrange / act / assert
            source.ComparesWith(candidate).Should().Be(expectation);
        }
    }
}