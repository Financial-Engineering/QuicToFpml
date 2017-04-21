using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/traderunningspread/fpml","GET")]
    [DataContract]
    public class TradeRunningSpreadFpml : TradeFpml, IReturn<TradeRunningSpreadFpmlResponse>
    {
    }

    [DataContract]
    public class TradeRunningSpreadFpmlResponse : TradeFpmlResponse
    {
    }
}