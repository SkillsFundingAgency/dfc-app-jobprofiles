using System;

namespace DFC.App.Services.Common.Converters
{
    /// <summary>
    /// I serialise types (contract).
    /// </summary>
    public interface ISerializeTypes
    {
        /// <summary>
        /// To string.
        /// </summary>
        /// <typeparam name="TConversion">The type being converted.</typeparam>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <returns>The converted type as a string.</returns>
        string ToString<TConversion>(TConversion typeToConvert);

        /// <summary>
        /// To string.
        /// </summary>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="classType">The class type.</param>
        /// <returns>The converted type as a string.</returns>
        string ToString(object typeToConvert, Type classType);

        /// <summary>
        /// From string.
        /// </summary>
        /// <typeparam name="TConversion">The type being converted.</typeparam>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <returns>The converted type.</returns>
        TConversion FromString<TConversion>(string typeToConvert);

        /// <summary>
        /// From string.
        /// </summary>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="classType">The class type.</param>
        /// <returns>The converted type.</returns>
        object FromString(string typeToConvert, Type classType);
    }
}
