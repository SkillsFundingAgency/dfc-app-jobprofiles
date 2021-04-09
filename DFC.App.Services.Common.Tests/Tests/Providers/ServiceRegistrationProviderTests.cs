using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Tests.Providers
{
    [TestClass]
    public sealed class ServiceRegistrationProviderTests :
        MoqTestingTests<ServiceRegistrationProvider, IProvideRegistrationServices>
    {
        private interface IConfigurationTest
        {
        }

        [TestMethod]
        public void ConstructorFailsWithNullAssemblyProviderThrows()
        {
            // arrange
            var config = MakeStrictMock<IConfiguration>();

            // act / assert
            Assert.ThrowsException<ArgumentNullException>(() => new ServiceRegistrationProvider(null, config));
        }

        [TestMethod]
        public void AddScopedMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IConvertJsonTypes), typeof(JsonTypeConverter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddScoped(services, registration);

            // assert
            VerifyAllMocks(sut);
            GetMock(services).VerifyAll();
        }

        [TestMethod]
        public void AddTransientMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IConvertJsonTypes), typeof(JsonTypeConverter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddTransient(services, registration);

            // assert
            VerifyAllMocks(sut);
            GetMock(services).VerifyAll();
        }

        [TestMethod]
        public void AddSingletonMeetsExpectation()
        {
            // arrange
            var sut = BuildTestSystem();

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var registration = new ExternalRegistrationAttribute(typeof(IConvertJsonTypes), typeof(JsonTypeConverter), TypeOfRegistrationScope.Scoped);

            // act
            sut.AddSingleton(services, registration);

            // assert
            VerifyAllMocks(sut);
            GetMock(services).VerifyAll();
        }

        [TestMethod]
        public void TestComposeMeetsExpectation()
        {
            // arrange
            // test is honed to exercise the configuration loading as there aren't any in this library.
            var sut = BuildTestSystem();
            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            var section = MakeStrictMock<IConfigurationSection>();
            GetMock(section)
                .SetupGet(x => x.Path)
                .Returns("any old path");
            GetMock(section)
                .SetupGet(x => x.Value)
                .Returns("any old value");
            GetMock(section)
                .Setup(x => x.GetChildren())
                .Returns(MakeEnumerableItems<IConfigurationSection>(0));

            GetMock(sut.Configuration)
                .Setup(x => x.GetSection("Configuration-Test"))
                .Returns(section);

            var assembly = GetAssemblyfor<ServiceRegistrationProvider>();
            GetMock(sut.Details)
                .Setup(x => x.Load())
                .Returns(MakeReadonlyItems<Assembly>(1, assembly));

            var registration = new ConfigurationRegistrationAttribute(typeof(IConfigurationTest), typeof(ConfigurationTest), "Configuration-Test");
            GetMock(sut.Details)
                .Setup(x => x.GetAttributeListFor<ConfigurationRegistrationAttribute>(assembly))
                .Returns(MakeReadonlyItems<ConfigurationRegistrationAttribute>(1, registration));
            GetMock(sut.Details)
                .Setup(x => x.GetAttributeListFor<ExternalRegistrationAttribute>(assembly))
                .Returns(MakeReadonlyItems<ExternalRegistrationAttribute>(0));
            GetMock(sut.Details)
                .Setup(x => x.GetAttributeListFor<InternalRegistrationAttribute>(assembly))
                .Returns(MakeReadonlyItems<InternalRegistrationAttribute>(0));

            // act
            sut.RegisterWith(services);

            // assert
            VerifyAllMocks(sut);
            GetMock(services).VerifyAll();
        }

        [TestMethod]
        public void RealSystem_ComposeMeetsExpectation()
        {
            // arrange
            var config = MakeStrictMock<IConfiguration>();
            var sut = ServiceRegistrar.Create(config);

            var services = MakeStrictMock<IServiceCollection>();
            GetMock(services)
                .Setup(x => x.Add(It.IsAny<ServiceDescriptor>()));

            // act
            sut.RegisterWith(services);

            // assert
            GetMock(config).VerifyAll();
            GetMock(services).Verify(
                x => x.Add(It.IsAny<ServiceDescriptor>()),
                Times.Exactly(9),
                "check for changes in the 'Registration/AssemblyRegistrations.cs' file");
        }

        internal override void VerifyAllMocks(ServiceRegistrationProvider sut)
        {
            GetMock(sut.Details).VerifyAll();
        }

        internal override ServiceRegistrationProvider BuildTestSystem()
        {
            var details = MakeStrictMock<IProvideRegistrationDetails>();
            var config = MakeStrictMock<IConfiguration>();

            return new ServiceRegistrationProvider(details, config);
        }

        private class ConfigurationTest :
            IConfigurationTest,
            IRequireConfigurationRegistration
        {
        }
    }
}
