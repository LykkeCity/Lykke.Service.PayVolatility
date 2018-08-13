using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator;
using Lykke.Service.PayVolatility.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Service.PayVolatility.Client
{
    public class CachedPayVolatilityClient : PayVolatilityClient
    {
        private const int CacheHours = 1;
        private IMemoryCache _memoryCache;
        
        public CachedPayVolatilityClient(IHttpClientGenerator httpClientGenerator) : base(httpClientGenerator)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
        
        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(DateTime date,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValue($"GetDailyVolatilitiesAsync_{date.ToString()}",
                () => base.GetDailyVolatilitiesAsync(date, cancellationToken));
        }
        
        /// <summary>
        /// Returns volatilities of the last date.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValue($"GetDailyVolatilitiesAsync",
                () => base.GetDailyVolatilitiesAsync(cancellationToken));
        }

        /// <summary>
        /// Returns volatility.
        /// </summary>
        /// <param name="date">Date of volatility.</param>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public Task<VolatilityModel> GetDailyVolatilityAsync(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValue($"GetDailyVolatilitiesAsync_{date.ToString()}",
                async () =>
                {
                    var volatilities = await base.GetDailyVolatilitiesAsync(date, cancellationToken);
                    return volatilities.FirstOrDefault(v =>
                        string.Equals(v.AssetPairId, assetPairId, StringComparison.OrdinalIgnoreCase));
                });
        }
        
        /// <summary>
        /// Returns volatility of the last day.
        /// </summary>
        /// <param name="date">Date of volatility.</param>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public Task<VolatilityModel> GetDailyVolatilityAsync(string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValue($"GetDailyVolatilitiesAsync",
                async () =>
                {
                    var volatilities = await base.GetDailyVolatilitiesAsync(cancellationToken);
                    return volatilities.FirstOrDefault(v =>
                        string.Equals(v.AssetPairId, assetPairId, StringComparison.OrdinalIgnoreCase));
                });
        }
        
        private async Task<T> GetCachedValue<T>(string key, Func<Task<T>> func) where T : class
        {
            T result = _memoryCache.Get<T>(key);
            if (result == null)
            {
                result = await func();
                _memoryCache.Set(key, result, TimeSpan.FromHours(CacheHours));
            }

            return result;
        }
    }
}
