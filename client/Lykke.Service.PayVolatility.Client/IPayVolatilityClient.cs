using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.PayVolatility.Models;
using Refit;

namespace Lykke.Service.PayVolatility.Client
{
    /// <summary>
    /// PayVolatility client interface.
    /// </summary>
    [PublicAPI]
    public interface IPayVolatilityClient
    {
        #region VolatilityController

        /// <summary>
        /// Returns volatilities of the specified date.
        /// </summary>
        /// <param name="date">Date of volatilities.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
        /// <returns code="200">Volatilities.</returns>
        /// <returns code="404">Volatilities for specified date is not found.</returns>
        /// <returns code="400">Input arguments are invalid.</returns>
        Task<IEnumerable<VolatilityModel>> GetDailyVolatilitiesAsync(DateTime date,
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
        Task<VolatilityModel> GetDailyVolatilityAsync(DateTime date, string assetPairId,
            CancellationToken cancellationToken = default(CancellationToken));

        #endregion VolatilityController
    }
}
