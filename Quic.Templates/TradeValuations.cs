using System.Collections.Generic;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{
    public class TradeValuations
    {
        public class TradeContext
        {
            public List<Trade> Trades { get; set; }
            public string[] Measures { get; set; }
        }

        //RequestValuationReport report { get; set; }
        public List<TradeContext> TradeContexts { get; set; }

        public TradeValuations(RequestValuationReport report)
        {
            TradeContexts = new List<TradeContext>();
            foreach (var tradeValuation in report.tradeValuationItem)
            {
                var context = new TradeContext
                {
                    Trades = new List<Trade>()
                };

                var parties = report.party.Select(party => party.partyId).ToList();
               // var party = parties.Select(p =>)


                foreach (var trade in tradeValuation.Items)
                    context.Trades.Add(new Trade(trade as FPML.Trade,"USD"));

                context.Measures =
                    (tradeValuation.valuationSet.quotationCharacteristics.Select(c => c.measureType.Value)).ToArray();

                TradeContexts.Add(context);
            }
        }

        public string ToCsv()
        {
            return "";
        }
    }
}