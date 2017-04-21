using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/tradepricing/csv","GET")]
    [DataContract]
    public class TradePricingCsv : TradeCsv, IReturn<TradePricingCsvResponse>
    {
    }

    [DataContract]
    public class TradePricingCsvResponse : TradeCsvResponse
    {
    }
}