using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/tradecva/fpml","GET")]
    [DataContract]
    public class TradeCvaFpml : TradeFpml, IReturn<TradeCvaFpmlResponse>
    {
        
    }

    [DataContract]
    public class TradeCvaFpmlResponse : TradeFpmlResponse
    {
    }

}