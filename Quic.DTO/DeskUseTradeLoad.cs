using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/deskusetradesave/{name}", "GET")]
    [DataContract]
    public class DeskUseTradeLoad : IReturn<DeskUseTradeLoadResponse>
    {

        [DataMember]
        public string SaveName = "";

    }

    [DataContract]
    public class DeskUseTradeLoadResponse : Response
    {
        [DataMember] 
        public string[] SavedNames;

        [DataMember]
        public TradeCsv.Trade[] Trades;
    }
}