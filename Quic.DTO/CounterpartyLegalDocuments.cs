using System;
using System.Runtime.Serialization;
using ServiceStack.ServiceHost;

namespace Quic.DTO
{
    [Route("/counterparty/{name}/legaldocuments","GET")]
    [DataContract]
    public class CounterpartyLegalDocuments : IReturn<CounterpartyLegalDocumentsResponse>
    {
        [DataMember]
        public string Name { get; set; }

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
    public class CounterpartyLegalDocumentsResponse : Response
    {
        [DataMember]
        public string[] Result { get; set; }
    }
}