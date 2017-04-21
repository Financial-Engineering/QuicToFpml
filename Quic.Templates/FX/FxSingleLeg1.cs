using System;
using System.Collections.Generic;
using Quic.FPML;

namespace Quic.Templates
{

    public partial class FxSingleLeg
    {
        string dtDelivery { get; set; }
        decimal rReceiveAmount { get; set; }
        string pYieldInfoReceive { get; set; }
        string pFXInfoReceive { get; set; }
        decimal rPayAmount { get; set; }
        string pYieldInfoPay { get; set; }
        string pFXInfoPay { get; set; }
        string strBusDayConv { get; set; }
        string strCurrency1 { get; set; }
        string strCurrency2 { get; set; }

        public FxSingleLeg()
        {            
        }

        public FxSingleLeg(FPML.Trade trade, IReadOnlyList<Tuple<string, string>> parties)
                    : base(trade, parties)
        {
            if (trade == null)
                return;

            var product = trade.Item as FPML.FxSingleLeg;

            if (product == null)
                return;

            strCurrency1      = product.exchangedCurrency1.paymentAmount.currency.Value;
            strCurrency2      = product.exchangedCurrency2.paymentAmount.currency.Value;

            dtDelivery        = Utilities.ToQuicDateFormat(product.Items1[0]);
            rReceiveAmount    = product.exchangedCurrency1.paymentAmount.amount;
            pYieldInfoReceive = Utilities.YieldCurve(strCurrency1);
            pFXInfoReceive    = Utilities.ExchangeCurve(strCurrency1);
            rPayAmount        = product.exchangedCurrency2.paymentAmount.amount;
            pYieldInfoPay     = Utilities.YieldCurve(strCurrency2);
            pFXInfoPay        = Utilities.ExchangeCurve(strCurrency2);
            strBusDayConv     = "NA";

        }
    }
}
