using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Quic.DateSchedule;
using Quic.FPML;
using BusinessDayConventionEnum = Quic.DateSchedule.BusinessDayConventionEnum;
using PeriodEnum = Quic.DateSchedule.PeriodEnum;
using RollConventionEnum = Quic.DateSchedule.RollConventionEnum;

namespace Quic.Templates
{

    public partial class SwapLeg
    {

        public string strNotionalExchange { get; set; }
        public string arNotional { get; set; }
        public string adtStart { get; set; }
        public string adtEnd { get; set; }
        public string adtPayment { get; set; }
        public string strCurrency { get; set; }
        public string strBusDayConv { get; set; }
        public string strDaycount { get; set; }
        public string strTradeId { get; set; }

        private bool swaptionOverride { get; set; }
        public bool xccyFlag { get; set; }

        public enum ExchangeEnum
        {
            NONE = 0x00,
            INITIAL = 0x01,
            FINAL = 0x02,
            BOTH = 0x03,
            INTERMEDIATE = 0x04
        }

        public static ExchangeEnum ToExchangeEnum(InterestRateStream swapLeg)
        {
            if (swapLeg.principalExchanges == null) 
                return ExchangeEnum.NONE;

            var bits = new BitVector32();
            var bit1 = BitVector32.CreateMask();
            var bit2 = BitVector32.CreateMask(bit1);
            var bit3 = BitVector32.CreateMask(bit2);

            bits[bit1] = swapLeg.principalExchanges.initialExchange;
            bits[bit2] = swapLeg.principalExchanges.finalExchange;
            bits[bit3] = swapLeg.principalExchanges.intermediateExchange;

            return (ExchangeEnum)bits.Data;
        }

        public SwapLeg(string strTradeId, bool xccy = false, bool swaptionOverride = false):base(strTradeId)
        {
            this.strTradeId = strTradeId;
            this.xccyFlag = xccy;
            this.swaptionOverride = swaptionOverride;
        }

        public virtual void CreateSwapLeg(InterestRateStream stream, IReadOnlyList<Tuple<string, string>> parties)
        {
            if (stream == null)
                return;

            var initialNotional = new decimal(0.0);

            var calculation = stream.calculationPeriodAmount.Item as Calculation;
            if (calculation != null)
            {
                var notional = calculation.Item as Notional;
                if (notional != null)
                {
                    strCurrency = notional.notionalStepSchedule.currency.Value;
                    initialNotional = notional.notionalStepSchedule.initialValue;
                }

                if (!StaticConfig.DayCountDictionary.ContainsKey(calculation.dayCountFraction.Value))
                    throw new ArgumentException(String.Format("Unknown daycount convention: {0}", calculation.dayCountFraction.Value));
                strDaycount = StaticConfig.DayCountDictionary[calculation.dayCountFraction.Value];
            }

            var side = stream.payerPartyReference.href == parties[0].Item1 ? "PAY" : "REC";
            strName = String.Format("{0}-{1}", strTradeId, side);

            strNotionalExchange = ToExchangeEnum(stream).ToString();

            var adjust = stream.calculationPeriodDates.calculationPeriodDatesAdjustments;

            if (adjust != null && adjust.businessDayConventionSpecified)
            {
                strBusDayConv = StaticConfig.BdcDictionary.ContainsKey(adjust.businessDayConvention)
                    ? StaticConfig.BdcDictionary[adjust.businessDayConvention]
                    : "NA";
            }

            if (stream.cashflows != null && stream.cashflows.cashflowsMatchParameters)
            {          
                arNotional =
                    String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                        .Select(calcPeriod => Convert.ToDouble(calcPeriod.Item)));

                adtStart =
                    String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                        .Select(calcPeriod => calcPeriod.adjustedStartDate.ToString("yyyy/MM/dd")));

                adtEnd =
                    String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                        .Select(calcPeriod => calcPeriod.adjustedEndDate.ToString("yyyy/MM/dd")));

                adtPayment =
                    String.Join(",", stream.cashflows.paymentCalculationPeriod.Select(period => period.adjustedPaymentDate.ToString("yyyy/MM/dd")));
            }
            else
            {
                var calcSchedule = CreateCalculationPeriodSchedule(stream.calculationPeriodDates);

                arNotional = String.Join(",", Enumerable.Repeat(initialNotional, calcSchedule.Periods.Count));

                adtStart = String.Join(",", calcSchedule.Select(d => d.StartDate.ToString("yyyy/MM/dd")));

                adtEnd = String.Join(",", calcSchedule.Select(d => d.EndDate.ToString("yyyy/MM/dd")));

                adtPayment = stream.paymentDates.payRelativeToSpecified &&
                             stream.paymentDates.payRelativeTo == PayRelativeToEnum.CalculationPeriodEndDate
                    ? adtEnd : adtStart;
            }
        }

        public DateSchedule.DateSchedule CreateCalculationPeriodSchedule(CalculationPeriodDates calcDates)
        {
            DateSchedule.DateSchedule calcSchedule = null;

 
            RollConventionEnum rollConv;

            try
            {
                rollConv = calcDates.calculationPeriodFrequency.rollConventionSpecified
                    ? (RollConventionEnum)
                        (Enum.Parse(typeof(RollConventionEnum),
                            calcDates.calculationPeriodFrequency.rollConvention.ToString()))
                    : RollConventionEnum.NONE;
            }
            catch 
            {
                rollConv = RollConventionEnum.NONE;
            }

            var multiplier = Int32.Parse(calcDates.calculationPeriodFrequency.periodMultiplier);

            var period =
                (PeriodEnum)(Enum.Parse(typeof(PeriodEnum),
                    calcDates.calculationPeriodFrequency.period));

            var stub = calcDates.stubPeriodTypeSpecified
                ? (StubEnum)Enum.Parse(typeof(StubEnum), calcDates.stubPeriodType.ToString())
                : StubEnum.NONE;

            var adjustableDate = calcDates.Item as AdjustableDate;
            if (adjustableDate != null)
            {
                var startDate = adjustableDate.unadjustedDate.Value;

                var startBdc = adjustableDate.dateAdjustments.businessDayConventionSpecified
                    ? (BusinessDayConventionEnum)(Enum.Parse(typeof(FPML.BusinessDayConventionEnum), adjustableDate.dateAdjustments.businessDayConvention.ToString()))
                    : BusinessDayConventionEnum.NONE;

                var startCals = Utilities.ConvertCalendars(adjustableDate.dateAdjustments.businessCenters != null
                    ? adjustableDate.dateAdjustments.businessCenters.businessCenter.Select(s => s.Value)
                    : new String[0]);

                var date = calcDates.Item1 as AdjustableDate;

                if (date != null)
                {
                    var endDate = date.unadjustedDate.Value;

                    var endBdc = date.dateAdjustments.businessDayConventionSpecified
                        ? (BusinessDayConventionEnum)(Enum.Parse(typeof(FPML.BusinessDayConventionEnum), date.dateAdjustments.businessDayConvention.ToString()))
                        : BusinessDayConventionEnum.NONE;

                    var endCals = Utilities.ConvertCalendars(date.dateAdjustments.businessCenters != null
                        ? date.dateAdjustments.businessCenters.businessCenter.Select(s => s.Value)
                        : new String[0]);

                    calcSchedule = new DateSchedule.DateSchedule(
                        new BusinessDate(startDate, new Calendar(startCals),startBdc),
                        new BusinessDate(endDate, new Calendar(endCals), endBdc), period, multiplier, rollConv, stub, startCals.Union(endCals).ToArray());
                }
            }

            return calcSchedule;
        }

        public override string GenerateSchedule()
        {
            return TransformText();
        }
    }
}