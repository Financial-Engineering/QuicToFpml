using System;
using System.Collections.Generic;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class EuropeanExerciseMap
    {
        public string strTradeId { get; set; }
        public string adtExpiry { get; set; }
        public string adtSettlement { get; set; }
        public string strPositionType { get; set; }
        public string strSettlementType { get; set; }
        public string strCurrency { get; set; }

        public EuropeanExerciseMap()
        {
            strTradeId = String.Empty;
        }

        public EuropeanExerciseMap(string tradeId, FPML.Swaption product, IReadOnlyList<Tuple<string, string>> parties) : base(tradeId)
        {
            strTradeId = tradeId;

            var premium = product.premium[0];

            strCurrency = premium.paymentAmount.currency.Value;
            strPositionType = product.buyerPartyReference.href == parties[0].Item1 ? "LONG" : "SHORT";
            strSettlementType = product.Item1 as SwaptionPhysicalSettlement != null ? "PHYSICAL" : "CASH";

            var euroExer = product.Item as EuropeanExercise;

            if (euroExer != null)
            {
                var adjustableDate = euroExer.expirationDate.Item as AdjustableDate;
                if (adjustableDate != null)
                    adtExpiry = adjustableDate.unadjustedDate.Value.ToString("yyyy/MM/dd");
            }

            adtSettlement = adtExpiry;
        }
    }

}
