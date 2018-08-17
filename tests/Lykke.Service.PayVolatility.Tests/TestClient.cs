using Lykke.HttpClientGenerator.Infrastructure;
using Lykke.Service.PayVolatility.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.PayVolatility.Tests
{
    public class TestClient
    {
        //[Fact]
        //public async Task Test1()
        //{
        //    var clientBuilder = HttpClientGenerator.HttpClientGenerator.BuildForUrl("http://pay-volatility.lykke-service.svc.cluster.local")
        //        .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

        //    clientBuilder = clientBuilder.WithoutRetries();

        //    var client = new CachedPayVolatilityClient(clientBuilder.Create(), new PayVolatilityServiceClientSettings(){ExpirationTimeUTC = DateTime.UtcNow.AddHours(1)});
        //    var result = await client.GetDailyVolatilitiesAsync(new DateTime(2018, 7, 31));
        //}
    }
}
