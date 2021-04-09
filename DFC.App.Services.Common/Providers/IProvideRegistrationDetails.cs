using System;
using System.Collections.Generic;
using System.Reflection;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// I provide registration details (contract).
    /// </summary>
    public interface IProvideRegistrationDetails
    {
        /// <summary>
        /// Load.
        /// </summary>
        /// <returns>A reaodnly collection of assembly.</returns>
        IReadOnlyCollection<Assembly> Load();

        /// <summary>
        /// Get the attribute list for...
        /// </summary>
        /// <typeparam name="TAttribute">The type of attribute.</typeparam>
        /// <param name="registrant">(the) registrant.</param>
        /// <returns>The attribute list.</returns>
        IReadOnlyCollection<TAttribute> GetAttributeListFor<TAttribute>(Assembly registrant)
            where TAttribute : Attribute;
    }
}