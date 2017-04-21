using System;
using System.Collections.Generic;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class SwapLegFixed
    {
        public string arFixedRate { get; set; }

        public SwapLegFixed(string tradeId, bool swaptionOverride = false)
            : base(tradeId, swaptionOverride)
        {           
        }

        public override void CreateSwapLeg(InterestRateStream stream, IReadOnlyList<Tuple<string, string>> parties)
        {
            base.CreateSwapLeg(stream, parties);

            if (stream.cashflows != null)
            {
                arFixedRate =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                            .Select(calcPeriod => calcPeriod.Item1));
            }
            else
            {
                var calculation = stream.calculationPeriodAmount.Item as Calculation;

                if (calculation != null)
                {
                    var schedule = calculation.Items[0] as Schedule;
                    if (schedule != null)
                    {
                        var fixedRate = schedule.initialValueSpecified ? schedule.initialValue : new decimal(0.0);
                        var len = arNotional.Split(',').Length; // TODO: UGLY!!! fix by changing properties to arrays?
                        arFixedRate = String.Join(",", Enumerable.Repeat(fixedRate, len));
                    }
                }          
            }
        }
    }
}
