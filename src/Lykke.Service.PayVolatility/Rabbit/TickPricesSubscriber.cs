using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Core.Settings;
using Lykke.Service.PayVolatility.Settings;

namespace Lykke.Service.PayVolatility.Rabbit
{
    public class TickPricesSubscriber : IStartable, IStopable
    {
        private readonly RabbitMqSubscriberSettings _settings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private readonly ICandlesRepository _candlesRepository;
        private readonly string[] _assetPairs;
        private RabbitMqSubscriber<TickPrice> _subscriber;

        public TickPricesSubscriber(RabbitMqSubscriberSettings settings,
            ILogFactory logFactory, ICandlesRepository candlesRepository,
            AssetPairSettings[] assetPairsSettings)
        {
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _settings = settings;
            _candlesRepository = candlesRepository;
            _assetPairs = assetPairsSettings.Select(p=>p.AssetPairId).ToArray();
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(
                    _settings.ConnectionString, 
                    _settings.ExchangeName, 
                    nameof(PayVolatility));

            _subscriber = new RabbitMqSubscriber<TickPrice>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        settings: settings,
                        logFactory: _logFactory,
                        retryTimeout: TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TickPrice>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding();

            _subscriber.Start();

            _log.Info($"<< {nameof(TickPricesSubscriber)} is started.");
        }

        public void Stop()
        {
            _subscriber?.Stop();

            _log.Info($"<< {nameof(TickPricesSubscriber)} is stopped.");
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private Task ProcessMessageAsync(TickPrice tickPrice)
        {
            if(!_assetPairs.Contains(tickPrice.Asset, StringComparer.OrdinalIgnoreCase))
                return Task.CompletedTask;

            return _candlesRepository.AddAsync(new Candle(tickPrice));
        }
    }
}
