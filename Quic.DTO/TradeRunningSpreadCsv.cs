using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/traderunningspread/csv","GET")]
    [DataContract]
    public class TradeRunningSpreadCsv : TradeCsv, IReturn<TradeRunningSpreadCsvResponse>
    {
    }

    [DataContract]
    public class TradeRunningSpreadCsvResponse : TradeCsvResponse
    {

    }

}