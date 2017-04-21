using System;
using System.Collections.Generic;
using System.Linq;
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

        public Swap(FPML.Trade trade, IReadOnlyList<Tuple<string, string>> parties)
            : base(trade, parties)
        {
            Legs = new List<SwapLeg>();

            strLeg1Name = String.Format("{0}-{1}", strTradeId, "REC");
            strLeg2Name = String.Format("{0}-{1}", strTradeId, "PAY");

            var product = trade.Item as FPML.Swap;

            if (product != null)
            {
                var xccyFlag = Utilities.IsXccySwap(product.swapStream);
                foreach (var interestRateStream in product.swapStream)
                {
                    var swapLeg = Utilities.IsFloat(interestRateStream)
                        ? (SwapLeg)new SwapLegFloat(strTradeId, xccyFlag)
                        : new SwapLegFixed(strTradeId);

                    swapLeg.CreateSwapLeg(interestRateStream,parties);

                    Legs.Add(swapLeg);
                }
            }
        }


        public override string GenerateSchedule()
        {
            return Legs.Aggregate("", (current, swapLeg) => current + swapLeg.TransformText());
        }
    }

}
