using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quic.DTO
{
    [DataContract]
    public class TradeResponse : Response
    {
        public TradeResponse()
        {
            Results = new List<TradeResult>();    
        }

        [DataMember]
        public List<TradeResult> Results { get; set; }

        [DataContract]
        public class TradeResult
        {
            [DataMember]
            public string TradeId { get; set; }

            [DataMember]
            public Dictionary<string, List<string>> Result { get; set; }
        }

    }
}