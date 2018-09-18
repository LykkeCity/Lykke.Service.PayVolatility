using JetBrains.Annotations;
using Lykke.Service.PayVolatility.Core.Settings;

namespace Lykke.Service.PayVolatility.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PayVolatilitySettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSubscriberSettings TickPricesSubscriber { get; set; }

        public VolatilityServiceSettings VolatilityService { get; set; }

        public AssetPairSettings[] AssetPairs { get; set; }
    }
}
