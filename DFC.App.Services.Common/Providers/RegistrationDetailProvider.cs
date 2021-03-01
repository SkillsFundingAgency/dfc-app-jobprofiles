using DFC.App.Services.Common.Converters;
using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DFC.App.Services.Common.Providers
{
    public sealed class RegistrationDetailProvider :
        IProvideRegistrationDetails,
        IRequireServiceRegistration
    {
        private const string _forAssemblies = @"participating_assemblies.json";

        public RegistrationDetailProvider(IProvideAssets assets, IConvertJsonTypes converter)
        {
            It.IsNull(assets)
                .AsGuard<ArgumentNullException>(nameof(assets));
            It.IsNull(converter)
                .AsGuard<ArgumentNullException>(nameof(converter));

            Assets = assets;
            Converter = converter;
        }

        internal IProvideAssets Assets { get; }

        internal IConvertJsonTypes Converter { get; }

        public IReadOnlyCollection<Assembly> Load()
        {
            var content = Assets.GetText(_forAssemblies);
            var list = Converter.FromString<List<string>>(content);
            return GetAssemblies(list);
        }

        public IReadOnlyCollection<TAttribute> GetAttributeListFor<TAttribute>(Assembly registrant)
            where TAttribute : Attribute
        {
            It.IsNull(registrant)
                .AsGuard<ArgumentNullException>(nameof(registrant));

            return registrant
                .GetCustomAttributes<TAttribute>()
                .AsSafeReadOnlyList();
        }

        internal IReadOnlyCollection<Assembly> GetAssemblies(IReadOnlyCollection<string> participatingFiles) =>
            participatingFiles
                .Select(x => Assembly.Load(new AssemblyName(x)))
                .AsSafeReadOnlyList();
    }
}