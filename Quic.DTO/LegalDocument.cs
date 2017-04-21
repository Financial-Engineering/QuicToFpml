using System;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{

    [Route("/legaldocument/{name}","GET")]
    [Route("/legaldocument/{name}/field/{field}","GET")]
    [DataContract]
    public class LegalDocument : IReturn<LegalDocumentResponse>
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public Boolean IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(Name);
            }
            set
            {
                
            }
        }
    }

    [DataContract]
    public class LegalDocumentResponse : Response
    {
        [DataMember]
        public string Result { get; set; }
    }

}
