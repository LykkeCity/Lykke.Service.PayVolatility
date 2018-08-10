using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Services
{
    public class VolatilityService : IVolatilityService
    {
        private readonly IVolatilityRepository _volatilityRepository;

        public VolatilityService(IVolatilityRepository volatilityRepository)
        {
            _volatilityRepository = volatilityRepository;
        }

        public Task<IEnumerable<IVolatility>> GetAsync(DateTime date)
        {
            return _volatilityRepository.GetAsync(date);
        }

        public Task<IVolatility> GetAsync(DateTime date, string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId))
            {
                throw new ArgumentNullException(nameof(assetPairId));
            }

            return _volatilityRepository.GetAsync(date, assetPairId);
        }
    }
}
