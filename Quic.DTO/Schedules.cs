using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [DataContract]
    [Route("/schedules","GET")]
    [Route("/schedules/{tradeId}", "GET")]
    public class Schedules : IReturn<SchedulesResponse>
    {
        [DataMember]
        public string TradeId { get; set; }
    }

    [DataContract]
    public class SchedulesResponse : Response
    {
        [DataMember]
        public string Result { get; set; }
    }
}