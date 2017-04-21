using System;
using System.Collections.Generic;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{

    public partial class FxOption
    {
        public string dtDelivery { get; set; }
        public decimal rReceiveAmount { get; set; }
        public string pYieldInfoReceive { get; set; }
        public string pFXInfoReceive { get; set; }
        public decimal rPayAmount { get; set; }
        public string pYieldInfoPay { get; set; }
        public string pFXInfoPay { get; set; }
        public string strBusDayConv { get; set; }
        public string putCurrency { get; set; }
        public string callCurrency { get; set; }
        public string settleCurrency { get; set; }
        public string dtContractStart { get; set; }
        public string dtContractEnd { get; set; }
        public string strPositionType { get; set; }
        public string rAccruedPayoff { get; set; }
        public string strPastExercise { get; set; }
        public string pYieldInfoSettle { get; set; }
        public string pFXInfoSettle { get; set; }
        public string pImpliedVol { get; set; }
        public string rSmileVolOverride { get; set; }

        public decimal putAmount { get; set; }
        public decimal callAmount { get; set; }

        // Single Barrier Properties
        public bool barrierOption { get; set; }
        public decimal rBarrier { get; set; }
        public string strBarrierType { get; set; }
        public string strBarrierCondition { get; set; }
        public decimal rRebate { get; set; }
        public string strRebatePaymentType { get; set; }
        public string strSamplingFrequency { get; set; }
        public int nViolation { get; set; }
        public string dtBarrierBreach { get; set; }

        // Double Barrier Properties
        public bool doubleBarrier { get; set; }
        public decimal rLBarrier { get; set; }
        public decimal rUBarrier { get; set; }
        public decimal rRebateLower { get; set; }
        public decimal rRebateUpper { get; set; }
        public decimal mpSolverPrefs { get; set; }

        public FxOption()
        {
            strProduct = "FXOptionVanillaEuropean";
        }

        public FxOption(FPML.Trade trade, IReadOnlyList<Tuple<string, string>> parties)
                    : base(trade, parties)
        {
            if (trade == null)
                return;

            var product = trade.Item as FPML.FxOption;

            if (product == null)
                return;

            var barrier = (product.features != null && product.features.Items != null) ? (product.features.Items[0] as FxBarrierFeature) : null;

            barrierOption = barrier != null;

            if (barrierOption)
            {
                doubleBarrier = product.features.Items.Length == 2;

                if (!doubleBarrier)
                {
                    strProduct = "FXSingleBarrierAnalytic";

                    bool invert = (product.callCurrencyAmount.currency.Value == barrier.quotedCurrencyPair.currency1.Value) && 
                        (barrier.quotedCurrencyPair.quoteBasis == QuoteBasisEnum.Currency2PerCurrency1);

                    var spot = invert ? 1/product.spotRate : product.spotRate;
                    rBarrier = invert ? 1/barrier.triggerRate : barrier.triggerRate;

                    strBarrierType = barrier.barrierType.ToString().ToUpper();
                    strBarrierCondition = rBarrier > spot ? "ABOVE" : "BELOW";
                    rRebate = new decimal(0.0);
                }
                else
                {
                   strProduct = "FXDoubleBarrierAnalytic";

                    var uBarrier = product.features.Items[1] as FxBarrierFeature;
                    rLBarrier = barrier.triggerRate;
                    rUBarrier = uBarrier.triggerRate;
                    strBarrierType = uBarrier.barrierType.ToString().ToUpper();
                    rRebateLower = new decimal(0.0);
                    rRebateUpper = new decimal(0.0);
                }

                strRebatePaymentType = "MATURITY";
                strSamplingFrequency = "CONTINUOUS";
                nViolation = 0;
                dtBarrierBreach = String.Empty;
            }

            putCurrency = product.putCurrencyAmount.currency.Value;
            callCurrency = product.callCurrencyAmount.currency.Value;

            dtContractStart = (product.effectiveDate.Item as AdjustableDate).adjustedDate.Value.ToString("yyyy/MM/dd");
            dtContractEnd = (product.Item as FxEuropeanExercise).expiryDate.ToString("yyyy/MM/dd");
            dtDelivery = (product.Item as FxEuropeanExercise).valueDate.ToString("yyyy/MM/dd");

            strPositionType = product.buyerPartyReference.href == parties[0].Item1 ? "BOUGHT" : "SOLD";

            rAccruedPayoff = "0";
            strPastExercise = String.Empty;

            rReceiveAmount = product.callCurrencyAmount.amount;
            pYieldInfoReceive = Utilities.YieldCurve(callCurrency,true);
            pFXInfoReceive = Utilities.ExchangeCurve(callCurrency);

            rPayAmount = product.putCurrencyAmount.amount;
            pYieldInfoPay = Utilities.YieldCurve(putCurrency,true);
            pFXInfoPay = Utilities.ExchangeCurve(putCurrency);

            settleCurrency = product.cashSettlement.settlementCurrency.Value;
            pYieldInfoSettle = Utilities.YieldCurve(settleCurrency);
            pFXInfoSettle = Utilities.ExchangeCurve(settleCurrency);

            pImpliedVol = Utilities.ImpliedVolCurve(callCurrency, putCurrency);

            rSmileVolOverride = String.Empty;
            strBusDayConv = "NA";
        }

    }
}
