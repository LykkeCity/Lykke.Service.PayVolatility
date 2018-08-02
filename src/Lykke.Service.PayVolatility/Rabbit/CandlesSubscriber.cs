using Autofac;
using AutoMapper;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.CandlesProducer.Contract;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Rabbit
{
    public class CandlesSubscriber : IStartable, IStopable
    {
        private readonly CandlesSubscriberSettings _settings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        private readonly ICandlesRepository _candlesRepository;
        private readonly IMapper _mapper;
        private readonly string[] _assetPairs;
        private RabbitMqSubscriber<CandlesUpdatedEvent> _subscriber;

        public CandlesSubscriber(CandlesSubscriberSettings settings, 
            ILogFactory logFactory, ICandlesRepository candlesRepository, 
            IMapper mapper, string[] assetPairs)
        {
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
            _settings = settings;
            _candlesRepository = candlesRepository;
            _mapper = mapper;
            _assetPairs = assetPairs;
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _settings.ConnectionString,
                QueueName = _settings.QueueName,
                ExchangeName = _settings.ExchangeName,
                IsDurable = false
            };

            _subscriber = new RabbitMqSubscriber<CandlesUpdatedEvent>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(
                        settings: settings,
                        logFactory: _logFactory,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new MessagePackMessageDeserializer<CandlesUpdatedEvent>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding();

            _subscriber.Start();

            _log.Info($"<< {nameof(CandlesSubscriber)} is started.");
        }

        public void Stop()
        {
            _subscriber?.Stop();

            _log.Info($"<< {nameof(CandlesSubscriber)} is stopped.");
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(CandlesUpdatedEvent @event)
        {
            var candleUpdates = @event.Candles.Where(c => c.PriceType == _settings.PriceType
                                                          && c.TimeInterval == _settings.TimeInterval
                                                          && _assetPairs.Contains(c.AssetPairId,
                                                              StringComparer.OrdinalIgnoreCase));

            var candles = _mapper.Map<IEnumerable<Candle>>(candleUpdates);
            var tasks = candles.Select(c => _candlesRepository.InsertAsync(c));

            await Task.WhenAll(tasks);
        }
    }
}
