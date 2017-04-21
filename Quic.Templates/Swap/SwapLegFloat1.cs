using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class SwapLegFloat
    {
        public string arLeverage;
        public string mpObservable;
        public string adtFixing { get; set; }
        public string arPastFixing { get; set; }

        public new string strType = "CouponFloat";

        public SwapLegFloat(FPML.Trade trade, InterestRateStream stream)
            : base(trade, stream)
        {

            var calculation = stream.calculationPeriodAmount.Item as Calculation;
            if (calculation != null)
            {
                var floatingRateCalculation = calculation.Items[0] as FloatingRateCalculation;
                if (floatingRateCalculation != null)
                {
                    var tenor = floatingRateCalculation.indexTenor.periodMultiplier +
                                floatingRateCalculation.indexTenor.period;
                    mpObservable = floatingRateCalculation.floatingRateIndex.Value + "-" + tenor;
                }
            }

            arLeverage =
                String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                    .Select(
                        calcPeriod =>
                        {
                            var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                            return floatingRateDefinition != null ? floatingRateDefinition.rateObservation[0].observationWeight : null;
                        }));

            arPastFixing =
                String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                .Select(
                calcPeriod =>
                {
                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                    return floatingRateDefinition != null ? floatingRateDefinition.rateObservation[0].observedRate : new decimal(0.0);
                }));


            adtFixing =
                String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                    .Select(
                        calcPeriod =>
                        {
                            var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                            return floatingRateDefinition != null ? floatingRateDefinition.rateObservation[0].adjustedFixingDate.ToString("yyyy/MM/dd") : null;
                        }));

        }
    }
}
