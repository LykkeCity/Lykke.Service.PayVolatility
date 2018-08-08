using System;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public interface ICandle
    {
        string AssetPairId { get; }

        DateTime CandleTimestamp { get; }

        decimal Open { get; }

        DateTime OpenTimestamp { get; set; }

        decimal Close { get; }

        DateTime CloseTimestamp { get; set; }

        decimal High { get; }

        decimal Low { get; }
    }
}
