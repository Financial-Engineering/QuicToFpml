using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/pricingmeasures/{instrument}","GET")]
    [DataContract]
    public class PricingMeasures : IReturn<PricingMeasuresResponse>
    {
        [DataMember]
        public string Instrument { get; set; }
    }

    [DataContract]
    public class PricingMeasuresResponse : Response
    {
        [DataMember]
        public string[] Result { get; set; }
    }
}