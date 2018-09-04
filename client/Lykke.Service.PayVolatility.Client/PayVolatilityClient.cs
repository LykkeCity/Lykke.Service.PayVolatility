using Lykke.HttpClientGenerator;

namespace Lykke.Service.PayVolatility.Client
{
    public class PayVolatilityClient : IPayVolatilityClient
    {
        public IVolatilityController Volatility { get; }
        
        public PayVolatilityClient(IHttpClientGenerator httpClientGenerator)
        {
            Volatility = httpClientGenerator.Generate<IVolatilityController>();
        }
    }
}
