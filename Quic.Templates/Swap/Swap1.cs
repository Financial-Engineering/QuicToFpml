using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quic.FPML;

namespace Quic.Templates
{


    public partial class Swap
    {
        public string strLeg1Name { get; set; }
        public string strLeg2Name { get; set; }

        public List<SwapLeg> Legs { get; set; } 

        public Swap()
        {
            Legs = new List<SwapLeg>();
        }

        public Swap(FPML.Trade trade, Dictionary<string, string> parties)
            : base(trade, parties)
        {
            Legs = new List<SwapLeg>();

            strLeg1Name = String.Format("{0}-{1}", strTradeId, "PAY");
            strLeg2Name = String.Format("{0}-{1}", strTradeId, "REC");

            var product = trade.product as FPML.Swap;

            if (product != null)
                foreach (var interestRateStream in product.swapStream)
                {
                    if (interestRateStream.calculationPeriodDates.id == "floatingCalcPeriodDates")
                        Legs.Add(new SwapLegFloat(trade, interestRateStream));
                    else
                        Legs.Add(new SwapLegFixed(trade, interestRateStream));
                }
        }

        public override string GenerateSchedule()
        {
            return Legs.Aggregate("", (current, swapLeg) => current + swapLeg.TransformText());
        }
    }

}
