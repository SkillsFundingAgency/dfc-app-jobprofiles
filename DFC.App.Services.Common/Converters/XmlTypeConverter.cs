using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace DFC.App.Services.Common.Converters
{
    /// <summary>
    /// XML type converter.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class XmlTypeConverter :
        IConvertXmlTypes,
        IRequireServiceRegistration
    {
        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public TConversion FromString<TConversion>(string typeToConvert)
        {
            It.IsNull(typeToConvert)
                .AsGuard<ArgumentNullException>(nameof(typeToConvert));

            return (TConversion)FromBytes(Encoding.UTF8.GetBytes(typeToConvert), typeof(TConversion));
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public object FromString(string typeToConvert, Type classType)
        {
            It.IsNull(typeToConvert)
                .AsGuard<ArgumentNullException>(nameof(typeToConvert));

            return FromBytes(Encoding.UTF8.GetBytes(typeToConvert), classType);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public string ToString<TConversion>(TConversion typeToConvert)
        {
            var decodedBytes = ToBytes(typeToConvert, typeof(TConversion));
            return Encoding.UTF8.GetString(decodedBytes, 0, decodedBytes.Length);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public string ToString(object typeToConvert, Type classType)
        {
            var decodedBytes = ToBytes(typeToConvert, classType);
            return Encoding.UTF8.GetString(decodedBytes, 0, decodedBytes.Length);
        }

        [ExcludeFromCodeCoverage]
        private static byte[] ToBytes(object item, Type type)
        {
            It.IsNull(item)
                .AsGuard<ArgumentNullException>();

            byte[] output;

            try
            {
                var stream = new MemoryStream();
                var settings = new XmlWriterSettings { Indent = true };
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    var serializer = new DataContractSerializer(type);
                    serializer.WriteObject(writer, item);
                }

                stream.Flush();
                output = stream.ToArray();
            }
            catch
            {
                throw new InvalidDataException(string.Format("the following type failed to serialise: {0}", type));
            }

            return output;
        }

        [ExcludeFromCodeCoverage]
        private static object FromBytes(byte[] item, Type classType)
        {
            It.IsNull(item)
                .AsGuard<ArgumentNullException>();

            object returnItem = null;

            using (var stream = new MemoryStream(item))
            {
                try
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        var serializer = new DataContractSerializer(classType);
                        returnItem = serializer.ReadObject(reader);
                    }
                }
                catch
                {
                    if (It.IsNull(returnItem))
                    {
                        throw new InvalidDataException(string.Format("the following type failed to deserialise: {0}", classType.Name));
                    }
                }
            }

            return returnItem;
        }
    }
}
