using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Core.Settings;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Service.PayVolatility.Services
{
    public class VolatilityCalculator: IStartable, IStopable
    {
        private const int CheckAndProcessPreviousDatesMinutesDelay = 1;
        private const int ChangesGap = 10;
        private readonly IVolatilityRepository _volatilityRepository;
        private readonly ICandlesRepository _candlesRepository;
        private readonly VolatilityServiceSettings _settings;
        private readonly string[] _assetPairs;
        private Timer _timer;
        private Timer _previousDatesTimer;
        private ILog _log;

        public VolatilityCalculator(ICandlesRepository candlesRepository,
            IVolatilityRepository volatilityRepository,
            VolatilityServiceSettings settings, 
            string[] assetPairs, ILogFactory logFactory)
        {
            _candlesRepository = candlesRepository;
            _volatilityRepository = volatilityRepository;
            _settings = settings;
            _assetPairs = assetPairs;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {            
            var calculateDateTime = DateTime.UtcNow.Date.Add(_settings.CalculateTime.TimeOfDay);
            if (DateTime.UtcNow.TimeOfDay >= _settings.CalculateTime.TimeOfDay)
            {
                calculateDateTime = calculateDateTime.AddDays(1);
            }

            _previousDatesTimer = new Timer(
                async (s) => await CheckAndProcessPreviousDates(calculateDateTime.Date.AddDays(-2)), 
                null,
                TimeSpan.FromMinutes(CheckAndProcessPreviousDatesMinutesDelay), 
                TimeSpan.FromMilliseconds(-1));

            _timer = new Timer(
                async (s) => await ProcessAsync(), 
                null, 
                calculateDateTime - DateTime.UtcNow,
                TimeSpan.FromDays(1));

            _log.Info($"Volatility service is started. Time for next processing is {calculateDateTime.ToString("s")}.");
        }

        private async Task CheckAndProcessPreviousDates(DateTime date)
        {
            _log.Info($"Start processing previous date: {date.ToString("yyyy-MM-dd")}.");

            var volatilities = (await _volatilityRepository.GetAsync(date)).ToArray();
            bool completedDate = true;
            foreach (string assetPair in _assetPairs)
            {
                if (volatilities.Select(v => v.AssetPairId).Contains(assetPair, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                bool processed = await ProcessAsync(assetPair, date);
                if (!processed)
                {
                    completedDate = false;
                }
            }
            _log.Info($"Finish processing previous date: {date.ToString("yyyy-MM-dd")}.");

            DateTime previousDate = date.AddDays(-1);
            if (!completedDate && previousDate >= DateTime.UtcNow.Date.AddDays(-_settings.ProcessingHistoryDepthDays))
            {
                await CheckAndProcessPreviousDates(date.AddDays(-1));
            }
        }

        private async Task ProcessAsync()
        {
            DateTime yesterday = DateTime.UtcNow.Date.AddDays(-1);

            _log.Info($"Start processing yesterday: {yesterday.ToString("yyyy-MM-dd")}.");

            foreach (string assetPair in _assetPairs)
            {
                await ProcessAsync(assetPair, yesterday);
            }
            _log.Info($"Finish processing yesterday: {yesterday.ToString("yyyy-MM-dd")}.");
        }

        private async Task<bool> ProcessAsync(string assetPair, DateTime date)
        {
            _log.Info($"Start processing {assetPair} {date.ToString("yyyy-MM-dd")}.");

            try
            {
                var candles = (await _candlesRepository.GetAsync(assetPair, date)).OrderBy(c=>c.CandleTimestamp).ToArray();
                if (!candles.Any())
                {
                    return false;
                }

                Volatility volatility = Calculate(candles);
                await _volatilityRepository.InsertAsync(volatility);
                await _candlesRepository.DeleteAsync(candles);
                _log.Info($"Processed {assetPair} {date.ToString("yyyy-MM-dd")} based on {candles.Length} candles.");
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Fail processing {assetPair} {date.ToString("yyyy-MM-dd")}.");
            }

            return true;
        }

        private Volatility Calculate(ICandle[] candles)
        {
            var closePriceChanges = new List<double>();
            var highPriceChanges = new List<double>();
            for (int i = ChangesGap; i < candles.Length; i++)
            {
                var baseIndex = i - ChangesGap;
                var baseCandle = candles[baseIndex];
                var candle = candles[i];
                decimal closePriceChange = candle.Close / baseCandle.Close - 1;
                decimal highPriceChange = candle.High / baseCandle.High - 1;
                closePriceChanges.Add((double)closePriceChange);
                highPriceChanges.Add((double)highPriceChange);
            }

            var closePriceStdev = (decimal)closePriceChanges.StandardDeviation();
            var highPriceStdev = (decimal)highPriceChanges.StandardDeviation();

            var temp = candles.First();
            return new Volatility()
            {
                AssetPairId = temp.AssetPairId,
                Date = temp.CandleTimestamp.Date,
                MultiplierFactor = _settings.MultiplierFactor,
                ClosePriceStdev = closePriceStdev,
                ClosePriceVolatilityShield = _settings.MultiplierFactor * closePriceStdev,
                HighPriceStdev = highPriceStdev,
                HighPriceVolatilityShield = _settings.MultiplierFactor * highPriceStdev
            };
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _previousDatesTimer?.Dispose();
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _log.Info("Volatility service is stopped.");
        }
    }
}
