using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [DataContract]
    [Route("/portfolio","GET")]
    [Route("/portfolio/{tradeId}","GET")]
    [Route("/portfolio/counterparty/{counterparty}","GET")]
    [Route("/portfolio/legaldocument/{legalDocument}","GET")]
    public class Portfolio : IReturn<PortfolioResponse>
    {
        [DataMember]
        public string TradeId { get; set; }
        [DataMember]
        public string Counterparty { get; set; }
        [DataMember]
        public string LegalDocument { get; set; }
    }

    [DataContract]
    public class PortfolioResponse : Response
    {

        [DataMember]
        public string[] Result { get; set; }
    }
}