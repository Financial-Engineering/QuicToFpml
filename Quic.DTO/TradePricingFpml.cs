using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/tradepricing/fpml","GET")]
    [DataContract]
    public class TradePricingFpml : TradeFpml, IReturn<TradePricingFpmlResponse>
    {
    }

    [DataContract]
    public class TradePricingFpmlResponse : TradeFpmlResponse
    {

    }

}