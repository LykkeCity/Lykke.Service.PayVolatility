using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.PayVolatility.Core.Domain;
using System;
using System.Globalization;

namespace Lykke.Service.PayVolatility.AzureRepositories.Volatility
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    public class VolatilityEntity : AzureTableEntity, IVolatility
    {
        private const string DateFormat = "yyyyMMdd";

        public DateTime Date => DateTime.ParseExact(PartitionKey,
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);

        public string AssetPairId => RowKey;

        private decimal _closePriceStdev;
        public decimal ClosePriceStdev
        {
            get => _closePriceStdev;
            set
            {
                _closePriceStdev = value;
                MarkValueTypePropertyAsDirty(nameof(ClosePriceStdev));
            }
        }

        private decimal _highPriceStdev;
        public decimal HighPriceStdev
        {
            get => _highPriceStdev;
            set
            {
                _highPriceStdev = value;
                MarkValueTypePropertyAsDirty(nameof(HighPriceStdev));
            }
        }

        private decimal _multiplierFactor;
        public decimal MultiplierFactor
        {
            get => _multiplierFactor;
            set
            {
                _multiplierFactor = value;
                MarkValueTypePropertyAsDirty(nameof(MultiplierFactor));
            }
        }

        private decimal _closePriceVolatilityShield;
        public decimal ClosePriceVolatilityShield
        {
            get => _closePriceVolatilityShield;
            set
            {
                _closePriceVolatilityShield = value;
                MarkValueTypePropertyAsDirty(nameof(ClosePriceVolatilityShield));
            }
        }

        private decimal _highPriceVolatilityShield;
        public decimal HighPriceVolatilityShield
        {
            get => _highPriceVolatilityShield;
            set
            {
                _highPriceVolatilityShield = value;
                MarkValueTypePropertyAsDirty(nameof(HighPriceVolatilityShield));
            }
        }

        public VolatilityEntity()
        {
        }

        public VolatilityEntity(IVolatility volatility)
        {
            PartitionKey = GetPartitionKey(volatility.Date);
            RowKey = GetRowKey(volatility.AssetPairId);
            ClosePriceStdev = volatility.ClosePriceStdev;
            HighPriceStdev = volatility.HighPriceStdev;
            MultiplierFactor = volatility.MultiplierFactor;
            ClosePriceVolatilityShield = volatility.ClosePriceVolatilityShield;
            HighPriceVolatilityShield = volatility.HighPriceVolatilityShield;
        }

        internal static string GetPartitionKey(DateTime date)
        {
            return date.ToString(DateFormat);
        }

        internal static string GetRowKey(string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId))
            {
                throw new ArgumentNullException(assetPairId);
            }

            return assetPairId;
        }
    }
}
