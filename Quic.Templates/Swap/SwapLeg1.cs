using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Quic.FPML;

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
        public string strName { get; set; }
        public string strType { get; set; }
        public string strBusDayConv { get; set; }
        public string strDaycount { get; set; }


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

            var bits = new BitVector32();
            var bit1 = BitVector32.CreateMask();
            var bit2 = BitVector32.CreateMask(bit1);
            var bit3 = BitVector32.CreateMask(bit2);

            bits[bit1] = swapLeg.principalExchanges.initialExchange;
            bits[bit2] = swapLeg.principalExchanges.finalExchange;
            bits[bit3] = swapLeg.principalExchanges.intermediateExchange;

            return (ExchangeEnum)bits.Data;
        }

        public SwapLeg()
        {          
        }

        public SwapLeg(FPML.Trade trade) : base(trade, new Dictionary<string, string>())
        {
            var product = trade.product as FPML.Swap;
            strName = (product.swapStream[0].payerPartyReference.href == "party1") 
                ? String.Format("{0}-{1}", strTradeId, "PAY") 
                : String.Format("{0}-{1}", strTradeId, "REC");

        }

        public SwapLeg(FPML.Trade trade, InterestRateStream swapLeg) : this(trade)
        {
            if (swapLeg == null)
                return;

            var calculation = swapLeg.calculationPeriodAmount.Item as Calculation;
            if (calculation != null)
            {
                var notional = calculation.Item as Notional;
                if (notional != null)
                    strCurrency = notional.notionalStepSchedule.currency.Value;

                strDaycount = DayCountDictionary[calculation.dayCountFraction.Value];
            }

            var side = swapLeg.payerPartyReference.href == "party1" ? "PAY" : "REC";
            strName = String.Format("{0}-{1}", strTradeId, side);

            strNotionalExchange = ToExchangeEnum(swapLeg).ToString();

            strBusDayConv = BdcDictionary[swapLeg.calculationPeriodDates.calculationPeriodDatesAdjustments.businessDayConvention];
            arNotional =
                String.Join(",", swapLeg.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                .Select(calcPeriod => Convert.ToDouble(calcPeriod.Item)));

            adtStart =
                String.Join(",", swapLeg.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                    .Select(calcPeriod => calcPeriod.adjustedStartDate.ToString("yyyy/MM/dd")));

            adtEnd =
                 String.Join(",", swapLeg.cashflows.paymentCalculationPeriod.Select(period => period.Items[0] as CalculationPeriod)
                    .Select(calcPeriod => calcPeriod.adjustedEndDate.ToString("yyyy/MM/dd")));

            adtPayment =
                String.Join(",", swapLeg.cashflows.paymentCalculationPeriod.Select(period => period.adjustedPaymentDate.ToString("yyyy/MM/dd")));
        }

        public override string GenerateSchedule()
        {
            return TransformText();
        }
    }
}