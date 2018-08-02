using System;

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
    }
}
