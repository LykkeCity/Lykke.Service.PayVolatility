using AzureStorage;
using Lykke.Service.PayVolatility.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.AzureRepositories.Volatility
{
    public class VolatilityRepository : IVolatilityRepository
    {
        private readonly INoSQLTableStorage<VolatilityEntity> _storage;

        public VolatilityRepository(INoSQLTableStorage<VolatilityEntity> storage)
        {
            _storage = storage;
        }

        public Task InsertAsync(IVolatility volatility)
        {
            return _storage.InsertOrMergeAsync(new VolatilityEntity(volatility));
        }

        public async Task<IEnumerable<IVolatility>> GetAsync(DateTime date)
        {            
            return await _storage.GetDataAsync(VolatilityEntity.GetPartitionKey(date));
        }

        public async Task<IVolatility> GetAsync(DateTime date, string assetPairId)
        {
            return await _storage.GetDataAsync(VolatilityEntity.GetPartitionKey(date),
                VolatilityEntity.GetRowKey(assetPairId));
        }
    }
}
