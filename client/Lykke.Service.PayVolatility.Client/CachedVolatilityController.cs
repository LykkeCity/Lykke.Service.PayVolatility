using Lykke.Common.Cache;
using Lykke.Service.PayVolatility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Client
{
    public class CachedVolatilityController : IVolatilityController
    {
        private const int CacheHours = 1;
        private readonly IVolatilityController _volatilityController;
        private readonly IPayVolatilityServiceClientCacheSettings _settings;
        private OnDemandDataCache<IEnumerable<VolatilityModel>> _memoryCache;

        public CachedVolatilityController(IVolatilityController volatilityController,
            IPayVolatilityServiceClientCacheSettings settings)
        {
            _volatilityController = volatilityController;
            _settings = settings;
            _memoryCache = new OnDemandDataCache<IEnumerable<VolatilityModel>>();
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
            return GetCachedValueAsync($"GetDailyVolatilitiesAsync_{date.ToString("yyyy-MM-dd")}",
                () => _volatilityController.GetDailyVolatilitiesAsync(date, cancellationToken));
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
            return GetCachedValueAsync("GetDailyVolatilitiesAsync",
                () => _volatilityController.GetDailyVolatilitiesAsync(cancellationToken));
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
        public async Task<VolatilityModel> GetDailyVolatilityAsync(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetCachedValueAsync($"GetDailyVolatilityAsync_{date.ToString("yyyy-MM-dd")}_{assetPairId}",
                    async () => new[]
                        {await _volatilityController.GetDailyVolatilityAsync(date, assetPairId, cancellationToken)}))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns volatility of the last day.
        /// </summary>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public async Task<VolatilityModel> GetDailyVolatilityAsync(string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await GetCachedValueAsync($"GetDailyVolatilityAsync_{assetPairId}",
                    async () => new[]
                        {await _volatilityController.GetDailyVolatilityAsync(assetPairId, cancellationToken)}))
                .FirstOrDefault();
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
                return _memoryCache.GetOrAddAsync(key, k => factory(), expiration);
            }

            if (_settings.CachePeriod.HasValue)
            {
                return _memoryCache.GetOrAddAsync(key, k => factory(), _settings.CachePeriod.Value);
            }

            return _memoryCache.GetOrAddAsync(key, k => factory(), TimeSpan.FromHours(CacheHours));
        }
    }
}
