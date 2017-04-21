using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/counterparty/{pattern}/regexpsearch","GET")]
    [DataContract]
    public class CounterpartyRegexpSearch : IReturn<CounterpartyRegexpSearchResponse>
    {
        [DataMember]
        public string Pattern { get; set; }
    }

    [DataContract]
    public class CounterpartyRegexpSearchResponse : Response
    {
        [DataMember]
        public string[] Result { get; set; }
    }
}