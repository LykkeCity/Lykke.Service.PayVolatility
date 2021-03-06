﻿using System;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Client 
{
    /// <summary>
    /// PayVolatility client settings.
    /// </summary>
    public class PayVolatilityServiceClientSettings : IPayVolatilityServiceClientCacheSettings
    {        
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}

        /// <summary>
        /// Absolute expiration time of the cached results, relative to now.
        /// If this is specified then ExpirationTimeUTC must be empty.
        /// If CachePeriod and ExpirationTimeUTC are empty then one hour is used as cache period.
        /// </summary>
        [Optional]
        public TimeSpan? CachePeriod { get; set; }

        /// <summary>
        /// UTC Time of the day of cache expiration.
        /// If this is specified then CachePeriod must be empty.
        /// If CachePeriod and ExpirationTimeUTC are empty then one hour is used as cache period.
        /// </summary>
        [Optional]
        public DateTime? ExpirationTimeUTC { get; set; }
    }
}
