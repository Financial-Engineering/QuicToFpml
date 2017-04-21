using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/observables","GET")]
    [Route("/observables/{name}","GET")]
    [Route("/observables/currency/{currency}","GET")]
    [DataContract]
    public class Observables : IReturn<ObservablesResponse>
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Currency { get; set; }
    }

    [DataContract]
    public class ObservablesResponse : Response
    {
        [DataMember]
        public string[] Result { get; set; }
    }

}