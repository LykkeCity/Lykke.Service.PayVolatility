using JetBrains.Annotations;

namespace Lykke.Service.PayVolatility.Client
{
    /// <summary>
    /// PayVolatility client interface.
    /// </summary>
    [PublicAPI]
    public interface IPayVolatilityClient
    {
        IVolatilityController Volatility { get; }
    }
}
