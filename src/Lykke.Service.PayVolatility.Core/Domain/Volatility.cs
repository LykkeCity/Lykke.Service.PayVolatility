using System;

namespace Lykke.Service.PayVolatility.Core.Domain
{
    public class Volatility : IVolatility
    {
        public string AssetPairId { get; set; }

        public DateTime Date { get; set; }

        public decimal ClosePriceStdev { get; set; }

        public decimal HighPriceStdev { get; set; }

        public decimal MultiplierFactor { get; set; }

        public decimal ClosePriceVolatilityShield { get; set; }

        public decimal HighPriceVolatilityShield { get; set; }
    }
}
