using System;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class SwapLegFixed
    {
        public string arFixedRate;

        public new string strType = "CouponFixed";

        public SwapLegFixed(FPML.Trade trade, InterestRateStream stream)
            : base(trade,stream)
        {
            arFixedRate =
                String.Join(",",
                    stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                        .Select(calcPeriod => calcPeriod.Item1));
        }
    }
}
