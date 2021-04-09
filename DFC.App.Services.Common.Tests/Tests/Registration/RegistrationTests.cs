using DFC.App.Services.Common.Providers;
using DFC.App.Services.Common.Registration;
using DFC.App.Services.Common.Registration.Attributes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace DFC.App.Services.Common.Tests.Registration
{
    /// <summary>
    /// Test (the) registrations in the module.
    /// This won't test the startup class
    /// but we now have some assurance that the registrations
    /// and the 'exported' classes are in sync.
    /// </summary>
    [TestClass]
    public sealed class RegistrationTests
    {
        [TestMethod]
        public void TestRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(AssetProvider));

            // act
            var types = assembly.GetTypes()
                .Where(x => x.IsClass && typeof(IRequireServiceRegistration).IsAssignableFrom(x));
            var registrations = assembly.GetCustomAttributes<InternalRegistrationAttribute>();

            //assert
            types.Count().Should().Be(registrations.Count());
        }

        [TestMethod]
        public void TestFaultRegistrations()
        {
            // arrange
            var assembly = Assembly.GetAssembly(typeof(AssetProvider));

            // act
            var registrations = assembly.GetCustomAttributes<FaultResponseRegistrationAttribute>();

            //assert
            registrations.Count().Should().Be(5);
        }
    }
}
