using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Client 
{
    /// <summary>
    /// PayVolatility client settings.
    /// </summary>
    public class PayVolatilityServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
