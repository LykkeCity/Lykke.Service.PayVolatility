using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string DataConnString { get; set; }

        public string CandlesTableName { get; set; }

        public string VolatilityTableName { get; set; }
    }
}
