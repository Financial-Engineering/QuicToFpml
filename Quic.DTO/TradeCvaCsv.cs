using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/tradecva/csv","GET")]
    [DataContract]
    public class TradeCvaCsv : TradeCsv, IReturn<TradeCvaCsvResponse>
    {
    }

    [DataContract]
    public class TradeCvaCsvResponse : TradeCsvResponse
    {
    }

}