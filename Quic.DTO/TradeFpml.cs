using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quic.DTO
{
    [DataContract]
    public abstract class TradeFpml
    {
        [DataMember]
        public string Fpml { get; set; }
        [DataMember]
        public Boolean NewTrade { get; set; }
        [DataMember]
        public bool StandAlone { get; set; }
    }

    [DataContract]
    public class TradeFpmlResponse : TradeResponse
    {
    }

}