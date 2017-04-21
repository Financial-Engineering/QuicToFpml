using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/trademtm/csv","GET")]
    [DataContract]
    public class TradeMtmCsv : TradeCsv, IReturn<TradeMtmCsvResponse>
    {
    }

    [DataContract]
    public class TradeMtmCsvResponse : TradeCsvResponse
    {
    }

}