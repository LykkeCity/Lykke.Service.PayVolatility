using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.PayVolatility.Core.Services;
using Lykke.Service.PayVolatility.Core.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Services
{
    public class CachedAssetsService : ICachedAssetsService
    {
        private readonly IAssetsService _assetsService;
        private readonly ConcurrentDictionary<string, AssetPair> _assetPairsCache;
        private readonly string[] _assetPairs;

        public CachedAssetsService(IAssetsService assetsService, 
            AssetPairSettings[] assetPairsSettings)
        {
            _assetsService = assetsService;
            _assetPairs = assetPairsSettings.Select(p=>p.AssetPairId).ToArray();
            _assetPairsCache = new ConcurrentDictionary<string, AssetPair>();
        }

        public async Task LoadAssetsAsync()
        {
            IList<AssetPair> assetPairs = await _assetsService.AssetPairGetAllAsync();

            foreach (var assetPair in assetPairs)
            {
                if (!_assetPairs.Contains(assetPair.Id, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                _assetPairsCache.AddOrUpdate(assetPair.Id.ToUpper(), assetPair, (k, a) => assetPair);
            }
        }

        public AssetPair GetAssetPair(string assetPairId)
        {
            if (!_assetPairsCache.TryGetValue(assetPairId.ToUpper(), out AssetPair assetPair))
            {
                throw new ArgumentException($"{assetPairId}) is not found.");
            }

            return assetPair;
        }
    }
}
