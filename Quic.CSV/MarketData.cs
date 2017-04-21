using System;
using System.IO;
using System.Linq;
using Quic.DateSchedule;

namespace Quic.CSV
{
    public class MarketData : CsvFile
    {
        public MarketData(string fname)
            : base(fname)
        {
            KeyIndex = 2; 
        }

        public override void Init()
        {
            if (CacheDictionary.ContainsKey(FileName)) return;

            InitWatcher();

            CacheDictionary[FileName] = new DocumentStructure();

            // Create a dictionary of trades keyed on cpty
            var lines = File.ReadAllLines(FileName);

            // By Curve Id
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.CurveId, 
                lines
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .Where(s => !s.StartsWith("//"))
                    .Select(line => line.Split(','))
                    .GroupBy(fields => String.Format("{0}.{1}.{2}", fields[1], fields[0], fields[3]))
                    .Select(group => new {@group.Key, g = @group})
                    .ToDictionary(item => item.Key, item => item.g.ToArray()[0]));

            // By Curve Type
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.CurveType, 
                ToDictionary(lines, 0));

            // By Currency
            CacheDictionary[FileName].NodeDictionaries.Add(DocumentStructure.FieldKeys.Currency,
                ToDictionary(lines, 3));
        }

        public DateTime GetValuationDate()
        {
            var result = GetBy(DocumentStructure.FieldKeys.CurveId, "USD.NumeraireRatio.USD");

            if (result.Length >= 2)
                return DateTime.Parse(result[2]);

            // If reference curve not found then return T-2
            return new BusinessDate(DateTime.Now).AddDays(-2);
        }

        public string[] GetByCurveName(string curveName)
        {
            return GetBy(DocumentStructure.FieldKeys.CurveId, curveName);
        }

        public string[] GetByCurveType(string curveType)
        {
            return GetBy(DocumentStructure.FieldKeys.CurveType, curveType);
        }

        public string[] GetByCurrency(string legalDocId)
        {
            return GetBy(DocumentStructure.FieldKeys.Currency, legalDocId);
        }
    }
}
