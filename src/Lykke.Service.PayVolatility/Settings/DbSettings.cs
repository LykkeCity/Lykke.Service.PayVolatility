using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
