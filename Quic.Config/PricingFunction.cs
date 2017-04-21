using System.Collections.Generic;

namespace Quic.Config
{
    public delegate DispatchResponse PricingFunction(PricingRequest request);

    public delegate DispatchResponse CvaFunction(List<PricingRequest> request);
}