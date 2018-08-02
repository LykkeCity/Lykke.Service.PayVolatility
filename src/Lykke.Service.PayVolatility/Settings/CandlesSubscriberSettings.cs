using Lykke.Job.CandlesProducer.Contract;

namespace Lykke.Service.PayVolatility.Settings
{
    public class CandlesSubscriberSettings : RabbitMqSubscriberSettings
    {
        public CandlePriceType PriceType { get; set; }

        public CandleTimeInterval TimeInterval { get; set; }
    }
}
