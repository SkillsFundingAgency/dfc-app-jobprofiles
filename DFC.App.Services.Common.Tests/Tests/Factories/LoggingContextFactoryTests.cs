using DFC.App.Services.Common.Factories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace DFC.App.Services.Common.Tests.Factories
{
    /// <summary>
    /// the context logging factory tests.
    /// this is a hard one as the microsoft logging classes are pretty much untestable...
    /// this service is the shimm offering us that testability in our higher level classes.
    /// </summary>
    [TestClass]
    public sealed class LoggingContextFactoryTests :
        MoqTestingTests<LoggingContextFactory, ICreateLoggingContexts>
    {
        [TestMethod]
        public void ConstructorFailsWithNullLoggingProvider()
        {
            // arrange / act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new LoggingContextFactory(null));
        }

        [TestMethod]
        [DataRow("context one...")]
        [DataRow("context two...")]
        public void BeginLoggingScopeForMeetsExpectation(string expectedContext)
        {
            // arrange
            var sut = BuildTestSystem();

            // can't make this strict, as extension methods are not supported / untestable...
            var logger = new Mock<ILogger>();
            /* extension method, not supported...
            logger
                .Setup(x => x.Log(LogLevel.Information, $"commencing logging for: '{expectedContext}'", null));
            */

            GetMock(sut.Factory)
                .Setup(x => x.CreateLogger(expectedContext))
                .Returns(logger.Object);

            // act
            var result = sut.BeginLoggingFor(expectedContext);

            // assert
            VerifyAllMocks(sut);

            result.Should().BeAssignableTo<ILoggingContextScope>();
            result.Should().BeAssignableTo<IDisposable>();
        }

        [TestMethod]
        [DataRow("context one...")]
        [DataRow("context two...")]
        public void LoggingContextScopeMeetsDisposableExpectation(string expectedContext)
        {
            // arrange
            // can't make this strict, as extension methods are not supported / untestable...
            var logger = new Mock<ILogger>();
            /* extension method, not supported...
            logger
                .Setup(x => x.Log(LogLevel.Information, $"commencing logging for: '{expectedContext}'", null));
            */

            // act
            using (var context = new LoggingContextFactory.LoggingContextScope(logger.Object, expectedContext))
            {
                // assert
                context.Should().BeAssignableTo<ILoggingContextScope>();
                context.Should().BeAssignableTo<IDisposable>();
            }
        }

        internal override LoggingContextFactory BuildTestSystem()
        {
            var provider = MakeStrictMock<ILoggerProvider>();

            return new LoggingContextFactory(provider);
        }

        internal override void VerifyAllMocks(LoggingContextFactory sut)
        {
            GetMock(sut.Factory).VerifyAll();
        }
    }
}