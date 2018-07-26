using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PayVolatilitySettings
    {
        public DbSettings Db { get; set; }
    }
}
