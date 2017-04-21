using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.Config;

namespace Quic.CSV
{
    public class Portfolio : CsvFile
    {

        private static string[] _lines;

        public bool StandAlone { get; set; }
        public HashSet<string> IgnoredTrades { get; set; }
 
        public Portfolio(string fname, bool standAlone = false)
            : base(fname)
        {
            KeyIndex = 2;
            StandAlone = standAlone;
            IgnoredTrades = new HashSet<string>();
        }

        public override void Init()
        {
            if (CacheDictionary.ContainsKey(FileName))
                return;

            InitWatcher();

            CacheDictionary[FileName] = new DocumentStructure();

            // Create a dictionary of trades keyed on cpty
            _lines = File.ReadAllLines(FileName);

            // By Counterparty
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.Counterparty, 
                ToDictionary(_lines, 2));

            // By Legal Document
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.LegalDocument,
                ToDictionary(_lines, 3));

            // By Trade Id
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.TradeId,
                ToDictionary(_lines, 0));
        }

        public IEnumerable<string> GetAll()
        {
            if (_lines == null)
                Init();

            return _lines;
        }

        public IEnumerable<string> GetByCounterparty(string cpId)
        {
            return GetBy(DocumentStructure.FieldKeys.Counterparty, cpId);
        }

        public IEnumerable<string> GetByTrade(string tradeId)
        {
            return GetBy(DocumentStructure.FieldKeys.TradeId, tradeId);
        }

        public IEnumerable<string> GetByLegalDocument(string legalDocId)
        {
            return GetBy(DocumentStructure.FieldKeys.LegalDocument, legalDocId);
        }

        public void WriteByLegalDocument(string fname, IEnumerable<string> legalDoc)
        {
            var trades = new List<string>();

            if (!StandAlone)
            {
                foreach (var doc in legalDoc)
                {
                    trades.AddRange(GetByLegalDocument(doc));
                }
            }

            var list = trades
                .Select(trade => new { trade, fields = trade.Split(',') })
                .Where(@t => !TemporaryItems.ContainsKey(@t.fields[0]))
                .Where(@t => !IgnoredTrades.Contains(@t.fields[0]))
                .Select(@t => @t.trade).Concat(TemporaryItems.Values);

            WriteRecords(fname, list);
        }

        public void WriteByCounterparty(string fname, IEnumerable<string> cpty)
        {
            if (cpty == null) 
                return;

            var trades = (cpty.SelectMany(GetByCounterparty)
                .Select(trade => new { trade, fields = trade.Split(',') })
                .Where(@t => !TemporaryItems.Keys.Contains(@t.fields[0]))
                .Select(@t => @t.trade)).Union(TemporaryItems.Values);

            WriteRecords(fname, trades);
        }

        public IEnumerable<string> GetLegs(string tradeId)
        {
          
            // TODO: Clean up
            var trade = String.Join(",", GetByTrade(tradeId));

            var legs = new Aggregation(ServiceConfig.Context.Aggregation)
                .RemoveFields(trade)
                .Split(',')
                .Where(leg => !String.IsNullOrEmpty(leg));

            return legs;
        }
    }
}