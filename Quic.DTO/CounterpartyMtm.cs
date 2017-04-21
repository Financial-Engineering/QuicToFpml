using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/counterpartymtm/{counterpartyName}", "GET")]
    [DataContract]
    public class CounterpartyMtm : CounterpartyBatch, IReturn<CounterpartyMtmResponse>
    {
        private CounterpartyMtm()
        {
            Measures = new[] { "CollectMTM" };
        }
    }

    [DataContract]
    public class CounterpartyMtmResponse : CounterpartyBatchResponse
    {
    }
}