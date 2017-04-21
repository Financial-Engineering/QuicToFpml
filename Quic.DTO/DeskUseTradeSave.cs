using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/deskusetradesave/{name}", "GET")]
    [DataContract]
    public class DeskUseTradeSave : IReturn<DeskUseTradeSaveResponse>
    {
        [DataMember] 
        public string SaveName;

        [DataMember] public bool OverRide;

        [DataMember] 
        public TradeCsv.Trade[] Trades;

    }

    [DataContract]
    public class DeskUseTradeSaveResponse : Response
    {
        [DataMember]
        public string Result;
    }
}