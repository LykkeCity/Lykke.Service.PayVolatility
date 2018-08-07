using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.PayVolatility.Core.Domain;
using System;
using System.Globalization;
using System.Linq;

namespace Lykke.Service.PayVolatility.AzureRepositories.Candles
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class CandleEntity : AzureTableEntity, ICandle
    {
        private const string TimestampFormat = "yyyy-MM-dd HH:mm";

        public string AssetPairId => PartitionKey.Split("_").First();

        public DateTime CandleTimestamp => DateTime.ParseExact(RowKey,
            TimestampFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);

        private decimal _open;
        public decimal Open
        {
            get => _open;
            set
            {
                _open = value;
                MarkValueTypePropertyAsDirty(nameof(Open));
            }
        }

        private decimal _close;
        public decimal Close
        {
            get => _close;
            set
            {
                _close = value;
                MarkValueTypePropertyAsDirty(nameof(Close));
            }
        }

        private decimal _high;
        public decimal High
        {
            get => _high;
            set
            {
                _high = value;
                MarkValueTypePropertyAsDirty(nameof(High));
            }
        }

        private decimal _low;
        public decimal Low
        {
            get => _low;
            set
            {
                _low = value;
                MarkValueTypePropertyAsDirty(nameof(Low));
            }
        }

        public CandleEntity()
        {
        }

        public CandleEntity(ICandle candle)
        {
            PartitionKey = GetPartitionKey(candle.AssetPairId, candle.CandleTimestamp);
            RowKey = GetRowKey(candle.CandleTimestamp);
            Open = candle.Open;
            Close = candle.Close;
            High = candle.High;
            Low = candle.Low;
        }

        internal static string GetPartitionKey(string assetPairId, DateTime date)
        {
            if (string.IsNullOrEmpty(assetPairId))
            {
                throw new ArgumentNullException(assetPairId);
            }

            return $"{assetPairId}_{date.ToString("yyyyMMdd")}";
        }

        internal static string GetRowKey(DateTime candleTimestamp)
        {
            return candleTimestamp.ToString(TimestampFormat);
        }
    }
}
