using DFC.App.Services.Common.Registration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Services.Common.Converters
{
    /// <summary>
    /// Json type converter (implementation).
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class JsonTypeConverter :
        IConvertJsonTypes,
        IRequireServiceRegistration
    {
        /// <summary>
        /// The settings.
        /// </summary>
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeConverter"/> class.
        /// </summary>
        public JsonTypeConverter()
        {
            _settings.Converters.Add(new StringEnumConverter());
        }

        /// <inheritdoc/>
        public TConversion FromString<TConversion>(string typeToConvert) =>
            (TConversion)FromString(typeToConvert, typeof(TConversion));

        /// <inheritdoc/>
        public object FromString(string typeToConvert, Type classType) =>
            JsonConvert.DeserializeObject(typeToConvert, classType, _settings);

        /// <inheritdoc/>
        public string ToString<TConversion>(TConversion typeToConvert) =>
            ToString(typeToConvert, typeof(TConversion));

        /// <inheritdoc/>
        public string ToString(object typeToConvert, Type classType) =>
            JsonConvert.SerializeObject(typeToConvert, classType, _settings);
    }
}