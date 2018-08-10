using System;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public interface IVolatility
    {
        string AssetPairId { get; }

        DateTime Date { get; }

        decimal ClosePriceStdev { get; }

        decimal HighPriceStdev { get; }

        decimal MultiplierFactor { get; }

        decimal ClosePriceVolatilityShield { get; }

        decimal HighPriceVolatilityShield { get; }
    }
}
