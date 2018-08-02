using System;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public interface ICandle
    {
        string AssetPairId { get; }

        DateTime CandleTimestamp { get; }

        decimal Open { get; }

        decimal Close { get; }

        decimal High { get; }

        decimal Low { get; }
    }
}
