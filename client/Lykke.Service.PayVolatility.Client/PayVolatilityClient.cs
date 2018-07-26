using Lykke.HttpClientGenerator;

namespace Lykke.Service.PayVolatility.Client
{
    public class PayVolatilityClient : IPayVolatilityClient
    {
        //public IControllerApi Controller { get; }
        
        public PayVolatilityClient(IHttpClientGenerator httpClientGenerator)
        {
            //Controller = httpClientGenerator.Generate<IControllerApi>();
        }
        
    }
}
