using System.IO;
using System.Threading.Tasks;

namespace DFC.App.Services.Common.Providers
{
    public interface IProvideAssets
    {
        Task<Stream> GetAsset(string assetName);

        Task<string> GetTextAsset(string assetName);

        string GetText(string assetName);
    }
}
