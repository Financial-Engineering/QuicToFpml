using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/counterparty/{name}","GET")]
    [Route("/counterparty/{name}/field/{field}","GET")]
    [DataContract]
    public class Counterparty : IReturn<CounterpartyResponse>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Field { get; set; }
    }

    [DataContract]
    public class CounterpartyResponse : Response
    {
        [DataMember]
        public string Result { get; set; }
    }
}