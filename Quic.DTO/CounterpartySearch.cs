using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/counterparty/{name}/search","GET")]
    [DataContract]
    public class CounterpartySearch : IReturn<CounterpartySearchResponse>
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class CounterpartySearchResponse : Response
    {

        [DataMember]
        public string[] Result { get; set; }
    }
}