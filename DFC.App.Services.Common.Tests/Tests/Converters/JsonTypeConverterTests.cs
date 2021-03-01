using DFC.App.Services.Common.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DFC.App.Services.Common.Tests.Converters
{
    [TestClass]
    public sealed class JsonTypeConverterTests :
        MoqTestingTests<JsonTypeConverter, IConvertJsonTypes>
    {
        private readonly TestItem testItemObject =
            new TestItem
            {
                EmailAddress = "an email address",
                Name = "a full name",
            };

        private readonly string testItemString = "{\"Name\":\"a full name\",\"EmailAddress\":\"an email address\"}";

        [TestMethod]
        public void ToStringMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.ToString(testItemObject);

            // assert
            result.Should().Be(testItemString);
        }

        [TestMethod]
        public void FromStringMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.FromString<TestItem>(testItemString);

            // assert
            result.EmailAddress.Should().Be(testItemObject.EmailAddress);
            result.Name.Should().Be(testItemObject.Name);
        }

        internal override JsonTypeConverter BuildTestSystem() =>
            new JsonTypeConverter();

        internal override void VerifyAllMocks(JsonTypeConverter sut)
        {
            // nothing to do...
        }

        private sealed class TestItem
        {
            public string Name { get; set; }

            public string EmailAddress { get; set; }
        }
    }
}