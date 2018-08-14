using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.HttpClientGenerator;
using Lykke.Service.PayVolatility.Models;

namespace Lykke.Service.PayVolatility.Client
{
    public class PayVolatilityClient : IPayVolatilityClient
    {
        public IVolatilityController VolatilityController { get; }
        
        public PayVolatilityClient(IHttpClientGenerator httpClientGenerator)
        {
            VolatilityController = httpClientGenerator.Generate<IVolatilityController>();
        }

        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public virtual Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(DateTime date,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return VolatilityController.GetDailyVolatilities(date, cancellationToken);
        }

        /// <summary>
        /// Returns volatilities of the last date.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public virtual Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return VolatilityController.GetDailyVolatilities(null, cancellationToken);
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
        public virtual Task<VolatilityModel> GetDailyVolatilityAsync(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return VolatilityController.GetDailyVolatility(date, assetPairId, cancellationToken);
        }

        /// <summary>
        /// Returns volatility of the last day.
        /// </summary>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        public virtual Task<VolatilityModel> GetDailyVolatilityAsync(string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return VolatilityController.GetDailyVolatility(null, assetPairId, cancellationToken);
        }
    }
}
