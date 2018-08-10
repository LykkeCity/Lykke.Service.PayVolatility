using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.PayVolatility.Settings
{
    public class RabbitMqSubscriberSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }

        public string QueueName { get; set; }
    }
}
