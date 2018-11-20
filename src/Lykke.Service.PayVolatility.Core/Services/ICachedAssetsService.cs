using System.Threading.Tasks;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.Service.PayVolatility.Core.Services
{
    public interface ICachedAssetsService
    {
        Task LoadAssetsAsync();
        AssetPair GetAssetPair(string assetPairId);
    }
}
