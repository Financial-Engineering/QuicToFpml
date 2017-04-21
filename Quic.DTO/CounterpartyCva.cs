using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{

    [Route("/counterpartycva/{counterpartyName}", "GET")]
    [Route("/counterpartycva/{counterpartyName}/measures/{measures}", "GET")]
    [DataContract]
    public class CounterpartyCva : CounterpartyBatch, IReturn<CounterpartyCvaResponse>
    {
    }

    [DataContract]
    public class CounterpartyCvaResponse : CounterpartyBatchResponse
    {
    }

}