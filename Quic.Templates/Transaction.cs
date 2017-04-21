using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Quic.FPML;

namespace Quic.Templates
{
    public class Transaction : IEnumerable<Trade>
    {

        private readonly List<Trade> _trades = new List<Trade>();

        public readonly IReadOnlyList<Tuple<string, string>> Parties;
        public readonly string[] Measures;
        public readonly DateTime ValuationDate;

        /// <summary>
        /// Main interface for FpML to CSV conversion
        /// </summary>
        /// <param name="fpml">FpML 5.3 Reporting Document</param>
        public Transaction(string fpml)
        {
            var report = Config.Utilities.Deserialize<RequestValuationReport>(fpml);

            Parties = report.party.Select(p => Tuple.Create(p.id, p.partyId[0].Value)).ToList();

            foreach (var tradeValuationItem in report.tradeValuationItem)
            {
                Measures = (tradeValuationItem.valuationSet.quotationCharacteristics.Select(c => c.measureType.Value)).ToArray();
                ValuationDate = tradeValuationItem.valuationSet.valuationScenario[0].valuationDate.Value;
                foreach (var trade in tradeValuationItem.Items)
                {
                    var trade1 = trade as FPML.Trade;

                    if (trade1 == null)
                        throw new ArgumentException("ERROR: Trade is empty for TradeValuation Item");

                    var product = trade1.Item;

                    if (trade1.documentation == null)
                        throw new ArgumentException(String.Format("ERROR: No Legal Agreement (Documentation tag) specified for: {0}", trade1.id));

                    var cls = String.Format("{0}.{1}", GetType().Namespace, trade1.Item.GetType().Name);

                    var type = Type.GetType(cls);

                    // Instantiate the appropriate template for the given type
                    if (type != null) 
                        _trades.Add(Activator.CreateInstance(type, new[] { trade, Parties}) as Trade);
                    else
                        throw new ArgumentException(String.Format("ERROR: No FpML to CSV Template exists for: {0}", cls));
                }
            }
        }

        public string GenerateTransaction()
        {
            return _trades.Aggregate("", (current, trade) => current + trade.GenerateTransaction());
        }

        public string GenerateSchedule()
        {
            return _trades.Aggregate("", (current, trade) => current + trade.GenerateSchedule());
        }

        public IEnumerator<Trade> GetEnumerator()
        {
            return _trades.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
