using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public interface ICandlesRepository
    {
        Task<IEnumerable<ICandle>> GetAsync(string assetPairId, DateTime candleTimestamp);
        Task DeleteAsync(IEnumerable<ICandle> candles);
        Task InsertAsync(ICandle candle);
    }
}
