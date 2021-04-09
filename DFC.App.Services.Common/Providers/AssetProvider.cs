using DFC.App.Services.Common.Helpers;
using DFC.App.Services.Common.Registration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    /// <summary>
    /// The asset manager (implementation).
    /// </summary>
    /// <seealso cref="IProvideAssets" />
    [ExcludeFromCodeCoverage]
    public sealed class AssetProvider :
        IProvideAssets,
        IRequireServiceRegistration
    {
        /// <inheritdoc/>
        public async Task<Stream> GetAsset(string assetName) =>
            await Task.Run(() =>
            {
                var path = GetAssetFile(assetName);
                return File.OpenRead(path);
            });

        /// <inheritdoc/>
        public async Task<string> GetTextAsset(string assetName)
        {
            using (var stream = await GetAsset(assetName))
            {
                return AsString(stream);
            }
        }

        /// <summary>
        /// gets the text asset.
        /// </summary>
        /// <param name="assetName">Name of the asset file.</param>
        /// <returns>
        /// a string representing the retrieved asset.
        /// </returns>
        public string GetText(string assetName)
        {
            var path = GetAssetFile(assetName);
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Gets the asset file name.
        /// </summary>
        /// <param name="usingAssetName">Name of the using asset.</param>
        /// <returns>The storage file.</returns>
        public string GetAssetFile(string usingAssetName)
        {
            return RunPathOptions(usingAssetName);
        }

        /// <summary>
        /// Runs the path options.
        /// </summary>
        /// <param name="usingAssetName">Using asset name.</param>
        /// <returns>The path options.</returns>
        public string RunPathOptions(string usingAssetName)
        {
            var storage = AppDomain.CurrentDomain.BaseDirectory;

            // 'traditional' asset path search (desktop, web and UWP <= even though it's dead)
            var candidatePath = Path.Combine(storage, "Assets", usingAssetName);
            (!File.Exists(candidatePath))
                .AsGuard<FileNotFoundException>(usingAssetName);

            return candidatePath;
        }

        /// <summary>
        /// As string.
        /// </summary>
        /// <param name="fromStream">From stream.</param>
        /// <returns>The string.</returns>
        internal string AsString(Stream fromStream)
        {
            using (var reader = new StreamReader(fromStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}