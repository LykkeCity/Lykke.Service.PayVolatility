using AzureStorage;
using Lykke.Service.PayVolatility.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.AzureRepositories.Candles
{
    public class CandlesRepository : ICandlesRepository
    {
        private readonly INoSQLTableStorage<CandleEntity> _storage;

        public CandlesRepository(INoSQLTableStorage<CandleEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<ICandle>> GetAsync(string assetPairId, DateTime candleTimestamp)
        {
            return await _storage.GetDataAsync(CandleEntity.GetPartitionKey(assetPairId, candleTimestamp));
        }

        public Task DeleteAsync(IEnumerable<ICandle> candles)
        {
            return _storage.DeleteAsync(candles.Select(c => new CandleEntity(c) {ETag = "*"}));
        }

        public Task InsertAsync(ICandle candle)
        {
            return _storage.InsertOrMergeAsync(new CandleEntity(candle));
        }
    }
}
