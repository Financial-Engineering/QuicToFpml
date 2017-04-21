using Quic.PricingContext.Schemas;

namespace Quic.Config
{
    public class DispatchResponse
    {
        public DispatchResult Result { get; set; }

        public enum StatusEnum
        {
            SUCCESS,
            FAILURE

        }

        public StatusEnum Status { get; set; }
        public string Error { get; set; }

        public DispatchResponse()
        {
            Status = StatusEnum.SUCCESS;
        }
    }
}