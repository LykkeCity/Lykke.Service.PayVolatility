using Lykke.Service.PayVolatility.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Client
{
    public interface IVolatilityController
    {
        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        [Get("/api/Volatility/GetDailyVolatilities/")]
        Task<IEnumerable<VolatilityModel>> GetDailyVolatilities(DateTime date,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns volatility.
        /// </summary>
        /// <param name="date">Date of volatility.</param>
        /// <param name="assetPairId">Identifier of the asset pair.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatility.</returns>
        /// <returns code="404">Volatility is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        [Get("/api/Volatility/GetDailyVolatility/")]
        Task<VolatilityModel> GetDailyVolatility(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
