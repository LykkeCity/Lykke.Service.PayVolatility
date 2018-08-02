using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public interface IVolatilityRepository
    {
        Task InsertAsync(IVolatility volatility);
        Task<IEnumerable<IVolatility>> GetAsync(DateTime date);
        Task<IVolatility> GetAsync(DateTime date, string assetPairId);
    }
}
