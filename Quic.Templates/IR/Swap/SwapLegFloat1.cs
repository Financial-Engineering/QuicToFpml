using System;
using System.Collections.Generic;
using System.Linq;
using Quic.Config;
using Quic.DateSchedule;
using Quic.FPML;
using BusinessDayConventionEnum = Quic.DateSchedule.BusinessDayConventionEnum;
using PeriodEnum = Quic.DateSchedule.PeriodEnum;

namespace Quic.Templates
{
    public partial class SwapLegFloat
    {
        public string arLeverage { get; set; }
        public string mpObservable { get; set; }
        public string adtFixing { get; set; }
        public string arPastFixing { get; set; }
        public string arSpread { get; set; }

        private bool bermudanOverride { get; set; }

        public SwapLegFloat(string tradeId, bool xccyFlag = false, bool bermudanOverride = false)
            : base(tradeId, xccyFlag, bermudanOverride)
        {
            this.bermudanOverride = bermudanOverride;
        }

        public override void CreateSwapLeg(InterestRateStream stream, IReadOnlyList<Tuple<string, string>> parties)
        {
            base.CreateSwapLeg(stream, parties);

            var calculation = stream.calculationPeriodAmount.Item as Calculation;

            if (calculation != null)
            {
                var floatingRateCalculation = calculation.Items[0] as FloatingRateCalculation;
                if (floatingRateCalculation != null)
                {
                    var tenor = string.Format("{0}{1}", floatingRateCalculation.indexTenor.periodMultiplier, floatingRateCalculation.indexTenor.period);
                    if (floatingRateCalculation.floatingRateIndex.Value != null)
                    {
                        var ccy = floatingRateCalculation.floatingRateIndex.Value.Substring(0,3); // only take first 3 chars (assume it's a ccy code)

                        // If the tenor doesn't match then default to 3M, if that doesn't exist then vomit
                        if (ServiceConfig.Observables.ContainsKey(new ObservablesMap.Key { Ccy = ccy, Tenor = tenor }))
                            mpObservable = ServiceConfig.Observables[new ObservablesMap.Key { Ccy = ccy, Tenor = tenor }].ToString();
                        else if (ServiceConfig.Observables.ContainsKey(new ObservablesMap.Key { Ccy = ccy, Tenor = "3M" }))
                            mpObservable = ServiceConfig.Observables[new ObservablesMap.Key { Ccy = ccy, Tenor = "3M" }].ToString();
                        else
                            throw new ArgumentException(String.Format("No Observables Defined for Currency {0} and Default Tenor 3M", ccy));
                    }

                    // If bermudan then replace "Swap" with "BermudanSwaption"
                    if (bermudanOverride)
                        mpObservable = mpObservable.Replace("Swap", "BermudanSwaption");
                }
            }

            if (stream.cashflows != null && stream.cashflows.cashflowsMatchParameters)
            {
                arLeverage =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                            .Select(
                                calcPeriod =>
                                {
                                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                                    return floatingRateDefinition != null
                                        ? floatingRateDefinition.rateObservation[0].observationWeight
                                        : null;
                                }));

                arPastFixing =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                            .Select(
                                calcPeriod =>
                                {
                                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                                    return floatingRateDefinition != null
                                        ? floatingRateDefinition.rateObservation[0].observedRate
                                        : new decimal(0.0);
                                }).TakeWhile(num => num != new decimal(0.0)));

                arSpread =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                            .Select(calcPeriod =>
                                {
                                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                                    return floatingRateDefinition != null && floatingRateDefinition.spreadSpecified
                                        ? floatingRateDefinition.spread
                                        : new decimal(0.0);
                                }).TakeWhile(num => num != new decimal(0.0)));

                adtFixing =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                            .Select(
                                calcPeriod =>
                                {
                                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                                    return floatingRateDefinition != null
                                        ? floatingRateDefinition.rateObservation[0].adjustedFixingDate.ToString(
                                            "yyyy/MM/dd")
                                        : null;
                                }));
            }
            else
            {
                var multiplier = Int32.Parse(stream.resetDates.fixingDates.periodMultiplier);

                var period = (PeriodEnum)(Enum.Parse(typeof(PeriodEnum), stream.resetDates.fixingDates.period.ToString()));

                var bdc = stream.resetDates.fixingDates.businessDayConventionSpecified
                    ? (BusinessDayConventionEnum)(Enum.Parse(typeof(FPML.BusinessDayConventionEnum), stream.resetDates.fixingDates.businessDayConvention.ToString()))
                    : BusinessDayConventionEnum.NONE;

                var cals = Utilities.ConvertCalendars(stream.resetDates.fixingDates.businessCenters != null
                    ? stream.resetDates.fixingDates.businessCenters.businessCenter.Select(s => s.Value)
                    : new String[0]);

                var dates = stream.resetDates.resetRelativeToSpecified &&
                                       stream.resetDates.resetRelativeTo == ResetRelativeToEnum.CalculationPeriodStartDate
                    ? adtStart.Split(',').Select(d => new BusinessDate(DateTime.Parse(d), new Calendar(cals), bdc)).ToArray()
                    : adtEnd.Split(',').Select(d => new BusinessDate(DateTime.Parse(d))).ToArray();

                var adjDates = dates.Select(date => date.Add(multiplier, period, bdc, new Calendar(cals))).ToList();

                arLeverage = String.Join(",", Enumerable.Repeat(1, adjDates.Count));

                arPastFixing = String.Empty;



                adtFixing = String.Join(",", adjDates.Select(dt => dt.ToString("yyyy/MM/dd")));

                arSpread = String.Empty;

                if (calculation != null)
                {
                    var floatingRateCalculation = calculation.Items[0] as FloatingRateCalculation;
                    if (floatingRateCalculation != null && floatingRateCalculation.spreadSchedule != null)
                    {
                        var spreadSchedule = floatingRateCalculation.spreadSchedule[0];
                        arSpread = String.Join(",", Enumerable.Repeat(spreadSchedule.initialValueSpecified ? spreadSchedule.initialValue : 0, adjDates.Count));
                    }
                }
            }

            if (stream.cashflows != null && !stream.cashflows.cashflowsMatchParameters)
            {
                arPastFixing =
                    String.Join(",",
                        stream.cashflows.paymentCalculationPeriod.Select(p => p.Items[0] as CalculationPeriod)
                            .Select(
                                calcPeriod =>
                                {
                                    var floatingRateDefinition = calcPeriod.Item1 as FloatingRateDefinition;
                                    return floatingRateDefinition != null
                                        ? floatingRateDefinition.rateObservation[0].observedRate
                                        : new decimal(0.0);
                                }).TakeWhile(num => num != new decimal(0.0)));
            }
        }
    }
}
