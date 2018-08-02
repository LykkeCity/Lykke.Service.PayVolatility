using Lykke.Service.PayVolatility.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Core.Services
{
    public interface IVolatilityService
    {
        Task<IEnumerable<IVolatility>> GetAsync(DateTime date);

        Task<IVolatility> GetAsync(DateTime date, string assetPairId);
    }
}
