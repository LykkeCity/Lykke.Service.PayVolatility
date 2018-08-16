using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Cache;
using Lykke.HttpClientGenerator;
using Lykke.Service.PayVolatility.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Lykke.Service.PayVolatility.Client
{
    public class CachedPayVolatilityClient : PayVolatilityClient
    {
        private const int CacheHours = 1;
        private OnDemandDataCache<IEnumerable<VolatilityModel>> _memoryCache;
        private IPayVolatilityServiceClientCacheSettings _settings;


        public CachedPayVolatilityClient(IHttpClientGenerator httpClientGenerator, 
            IPayVolatilityServiceClientCacheSettings settings) : base(httpClientGenerator)
        {
            _memoryCache = new OnDemandDataCache<IEnumerable<VolatilityModel>>();
            _settings = settings;
        }
        
        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public override Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(DateTime date,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValueAsync($"GetDailyVolatilitiesAsync_{date.ToString("yyyy-MM-dd")}",
                () => base.GetDailyVolatilitiesAsync(date, cancellationToken));
        }
        
        /// <summary>
        /// Returns volatilities of the last date.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public override Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetCachedValueAsync("GetDailyVolatilitiesAsync",
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
        public override async Task<VolatilityModel> GetDailyVolatilityAsync(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetDailyVolatilitiesAsync(date, cancellationToken)).FirstOrDefault(v =>
                string.Equals(v.AssetPairId, assetPairId, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Returns volatility of the last day.
        /// </summary>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public override async Task<VolatilityModel> GetDailyVolatilityAsync(string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetDailyVolatilitiesAsync(cancellationToken)).FirstOrDefault(v =>
                string.Equals(v.AssetPairId, assetPairId, StringComparison.OrdinalIgnoreCase));
        }
        
        private Task<IEnumerable<VolatilityModel>> GetCachedValueAsync(string key, Func<Task<IEnumerable<VolatilityModel>>> factory)
        {
            if (_settings.ExpirationTimeUTC.HasValue)
            {
                DateTimeOffset expiration = DateTime.UtcNow.Date.Add(_settings.ExpirationTimeUTC.Value.TimeOfDay);
                if (expiration <= DateTime.UtcNow)
                {
                    expiration = expiration.AddDays(1);
                }
                return _memoryCache.GetOrAddAsync(key, k=> factory(), expiration);
            }

            if (_settings.CachePeriod.HasValue)
            {
                return _memoryCache.GetOrAddAsync(key, k => factory(), _settings.CachePeriod.Value);
            }

            return _memoryCache.GetOrAddAsync(key, k => factory(), TimeSpan.FromHours(CacheHours));
        }
    }
}
