using JetBrains.Annotations;
using Lykke.Service.PayVolatility.Core.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PayVolatilitySettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSubscriberSettings TickPricesSubscriber { get; set; }

        public VolatilityServiceSettings VolatilityService { get; set; }

        public string[] AssetPairs { get; set; }
    }
}
