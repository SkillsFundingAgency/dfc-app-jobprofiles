using DFC.App.Services.Common.Converters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization;

namespace DFC.App.Services.Common.Tests.Converters
{
    [TestClass]
    public sealed class XmlTypeConverterTests :
        MoqTestingTests<XmlTypeConverter, IConvertXmlTypes>
    {
        private readonly string testItemString = File.ReadAllText($"Assets{Path.DirectorySeparatorChar}TestItem.xml");

        private readonly TestItem testItemObject =
            new TestItem
            {
                EmailAddress = "an email address",
                Name = "a full name",
            };

        /*
        // TODO: this test needs fixing, there's a control character inducing a failure
        [TestMethod]
        public void ToStringMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            // act
            var result = sut.ToString(testItemObject);

            // assert
            Assert.AreEqual(result, testItemString);
        }
        */

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

        internal override XmlTypeConverter BuildTestSystem() =>
            new XmlTypeConverter();

        internal override void VerifyAllMocks(XmlTypeConverter sut)
        {
            // nothing to do...
        }

        [DataContract(Namespace = "not specified")]
        private sealed class TestItem
        {
            [DataMember]
            public string Name { get; set; }

            [DataMember]
            public string EmailAddress { get; set; }
        }
    }
}