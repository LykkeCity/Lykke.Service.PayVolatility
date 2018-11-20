using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.Assets.Client;

namespace Lykke.Service.PayVolatility.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public PayVolatilitySettings PayVolatilityService { get; set; }

        public AssetServiceSettings AssetService { get; set; }
    }
}
