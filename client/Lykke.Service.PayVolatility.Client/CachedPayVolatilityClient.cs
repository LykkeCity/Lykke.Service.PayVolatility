using Lykke.HttpClientGenerator;

namespace Lykke.Service.PayVolatility.Client
{
    public class CachedPayVolatilityClient : IPayVolatilityClient
    {
        public IVolatilityController Volatility { get; }

        public CachedPayVolatilityClient(IHttpClientGenerator httpClientGenerator,
            IPayVolatilityServiceClientCacheSettings settings)
        {
            Volatility =
                new CachedVolatilityController(httpClientGenerator.Generate<IVolatilityController>(), 
                    settings);
        }
    }
}
