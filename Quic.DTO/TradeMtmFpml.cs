using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/trademtm/fpml","GET")]
    [DataContract]
    public class TradeMtmFpml : TradeFpml, IReturn<TradeMtmFpmlResponse>
    {
    }

    [DataContract]
    public class TradeMtmFpmlResponse : TradeFpmlResponse
    {
    }
}