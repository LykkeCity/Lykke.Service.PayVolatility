using System;

namespace Lykke.Service.PayVolatility.Client
{
    public interface IPayVolatilityServiceClientCacheSettings
    {
        /// <summary>
        /// Absolute expiration time of the cached results, relative to now.
        /// If this is specified then ExpirationTimeUTC must be empty.
        /// If CachePeriod and ExpirationTimeUTC are empty then one hour is used as cache period.
        /// </summary>
        TimeSpan? CachePeriod { get; }

        /// <summary>
        /// UTC Time of the day of cache expiration.
        /// If this is specified then CachePeriod must be empty.
        /// If CachePeriod and ExpirationTimeUTC are empty then one hour is used as cache period.
        /// </summary>
        DateTime? ExpirationTimeUTC { get; }
    }
}
