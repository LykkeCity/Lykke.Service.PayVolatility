using System;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Core.Settings
{
    public class VolatilityServiceSettings
    {
        private const int DefaultProcessingHistoryDepthDays = 7;

        public VolatilityServiceSettings()
        {
            ProcessingHistoryDepthDays = DefaultProcessingHistoryDepthDays;
        }

        public decimal MultiplierFactor { get; set; }

        public DateTime CalculateTime { get; set; }

        [Optional]
        public int ProcessingHistoryDepthDays { get; set; }
    }
}
