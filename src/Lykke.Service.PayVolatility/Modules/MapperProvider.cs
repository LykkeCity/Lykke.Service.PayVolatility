using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AutoMapper.Configuration;
using Lykke.Job.CandlesProducer.Contract;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Models;

namespace Lykke.Service.PayVolatility.Modules
{
    public class MapperProvider
    {
        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();

            CreateRabbitMaps(mce);
            CreateVolatilityControllerMaps(mce);

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc);
        }

        private void CreateRabbitMaps(MapperConfigurationExpression mce)
        {
            mce.CreateMap<CandleUpdate, Candle>(MemberList.Destination);
        }

        private void CreateVolatilityControllerMaps(MapperConfigurationExpression mce)
        {
            mce.CreateMap<IVolatility, VolatilityModel>(MemberList.Destination);
        }
    }
}
