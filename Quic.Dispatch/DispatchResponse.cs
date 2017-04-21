using System.Collections.Generic;

namespace Quic.Dispatch
{
    public class DispatchResponse
    {
        public Dictionary<string,List<string>> Result { get; set; }

        public enum StatusEnum
        {
            Success,
            Failure
        }

        public StatusEnum Status { get; set; }
        public string Error { get; set; }

        public DispatchResponse()
        {
            Status = StatusEnum.Success;
        }
    }
}