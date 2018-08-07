using Autofac;
using AutoMapper;
using AzureStorage.Tables;
using Common;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Service.PayVolatility.AzureRepositories.Candles;
using Lykke.Service.PayVolatility.AzureRepositories.Volatility;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Core.Services;
using Lykke.Service.PayVolatility.Filters;
using Lykke.Service.PayVolatility.Rabbit;
using Lykke.Service.PayVolatility.Services;
using Lykke.Service.PayVolatility.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.PayVolatility.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            var mapperProvider = new MapperProvider();
            IMapper mapper = mapperProvider.GetMapper();
            builder.RegisterInstance(mapper).As<IMapper>();

            builder.Register(c =>
                    new CandlesRepository(AzureTableStorage<CandleEntity>.Create(
                    _appSettings.ConnectionString(x => x.PayVolatilityService.Db.DataConnString),
                    _appSettings.CurrentValue.PayVolatilityService.Db.CandlesTableName,
                    c.Resolve<ILogFactory>())))
                .As<ICandlesRepository>()
                .SingleInstance();

            builder.Register(c =>
                    new VolatilityRepository(AzureTableStorage<VolatilityEntity>.Create(
                        _appSettings.ConnectionString(x => x.PayVolatilityService.Db.DataConnString),
                        _appSettings.CurrentValue.PayVolatilityService.Db.VolatilityTableName,
                        c.Resolve<ILogFactory>())))
                .As<IVolatilityRepository>()
                .SingleInstance();

            builder.RegisterType<TickPricesSubscriber>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("settings", _appSettings.CurrentValue.PayVolatilityService.TickPricesSubscriber)
                .WithParameter("assetPairs", _appSettings.CurrentValue.PayVolatilityService.AssetPairs);

            builder.RegisterType<VolatilityCalculator>()
                .As<IStartable>()
                .As<IStopable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("settings", _appSettings.CurrentValue.PayVolatilityService.VolatilityService)
                .WithParameter("assetPairs", _appSettings.CurrentValue.PayVolatilityService.AssetPairs);

            builder.RegisterType<VolatilityService>()
                .As<IVolatilityService>()
                .SingleInstance();

            builder.RegisterType<ValidateActionParametersFilterAttribute>();
        }
    }
}
