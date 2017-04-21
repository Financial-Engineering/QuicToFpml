using System.Runtime.Serialization;

namespace Quic.DTO
{
    [DataContract]
    public abstract class Response
    {
        public enum StatusEnum
        {
            Success,
            Failure          
        }
        
        [DataMember]
        public StatusEnum Status { get; set; }

        [DataMember]
        public string Error { get; set; }
    }
}