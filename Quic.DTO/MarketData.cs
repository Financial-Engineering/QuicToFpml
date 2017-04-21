using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/marketdata/{curveName}","GET")]
    [Route("/marketdata/currency/{currency}","GET")]
    [Route("/marketdata/curvetype/{curveType}","GET")]
    [DataContract]
    public class MarketData : IReturn<MarketDataResponse>
    {
        [DataMember]
        public string CurveName { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string CurveType { get; set; }
    }

    [DataContract]
    public class MarketDataResponse : Response
    {
        [DataMember]
        public string[] Result { get; set; }
    }
}