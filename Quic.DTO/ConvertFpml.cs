using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/convertfpml", "GET")]
    [DataContract]
    public class ConvertFpml : TradeFpml,IReturn<ConvertFpmlResponse>
    {
        
    }

    [DataContract]
    public class ConvertFpmlResponse : Response
    {
        [DataMember]
        public Dictionary<string, string> Result { get; set; }
    }
}