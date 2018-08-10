using System;

namespace Lykke.Service.PayVolatility.Core.Settings
{
    public class VolatilityServiceSettings
    {
        public decimal MultiplierFactor { get; set; }
        public DateTime CalculateTime { get; set; }
    }
}
