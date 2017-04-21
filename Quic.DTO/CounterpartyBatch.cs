using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quic.DTO
{
    [DataContract]
    public abstract class CounterpartyBatch
    {
        [DataMember]
        public string CounterpartyName { get; set; }
        [DataMember]
        public string LegalDocumentName { get; set; }
        [DataMember]
        public string[] Measures { get; set; }
    }

    [DataContract]
    public class CounterpartyBatchResponse : Response
    {
        [DataMember]
        public Dictionary<string, List<string>> Result { get; set; }
    }
}