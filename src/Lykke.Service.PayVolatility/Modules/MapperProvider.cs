using AutoMapper;
using AutoMapper.Configuration;
using Lykke.Service.PayVolatility.Core.Domain;
using Lykke.Service.PayVolatility.Models;

namespace Lykke.Service.PayVolatility.Modules
{
    public class MapperProvider
    {
        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();

            CreateVolatilityControllerMaps(mce);

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc);
        }

        private void CreateVolatilityControllerMaps(MapperConfigurationExpression mce)
        {
            mce.CreateMap<IVolatility, VolatilityModel>(MemberList.Destination);
        }
    }
}
