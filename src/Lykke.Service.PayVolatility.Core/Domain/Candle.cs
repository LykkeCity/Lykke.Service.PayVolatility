using System;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public class Candle : ICandle
    {
        public string AssetPairId { get; set; }

        public DateTime CandleTimestamp { get; set; }

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public Candle()
        {
        }

        public Candle(TickPrice tickPrice)
        {
            AssetPairId = tickPrice.Asset;
            CandleTimestamp = tickPrice.Timestamp;
            High = Low = Open = Close = (tickPrice.Ask + tickPrice.Bid) / 2;
        }
    }
}
