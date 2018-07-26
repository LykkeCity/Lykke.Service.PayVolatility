using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Service.PayVolatility.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public PayVolatilitySettings PayVolatilityService { get; set; }        
    }
}
