using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.Config;

namespace Quic.CSV
{
    /// <summary>
    /// Represents the Aggregation.csv format
    /// </summary>
    public class ExportParams : CsvFile
    {
        private static new Dictionary<string, HashSet<string>> CacheDictionary { get; set; }

        static ExportParams()
        {
            CacheDictionary = new Dictionary<string, HashSet<string>>();
        }

        public ExportParams(string fname):this()
        {
            FileName = fname;
        }

        private ExportParams()
        {
        }

        /// <summary>
        /// Loads cache dictionary with aggregation hierarchy
        /// </summary>
        public override void Init()
        {
            if (CacheDictionary.ContainsKey(FileName)) return;

            InitWatcher();

            // read fields after Plugins:
            var list = File.ReadAllLines(FileName)
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Where(s => ! s.StartsWith("//"))
                .SkipWhile(s => !s.StartsWith("Plugins:"))
                .Skip(1)
                .Select(s => s.TrimEnd(',')).ToHashSet();

            // Explicit add of GRD since it is always exported so not necessarily defined
            // in the export file
            list.Add("GlobalReportDates");
            
            CacheDictionary[FileName] = list;
        }

        public override bool ContainsKey(string key)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return CacheDictionary[FileName].Contains(key);
        }

        public int Count
        {
            get
            {
                if (!CacheDictionary.ContainsKey(FileName))
                    Init();

                return CacheDictionary[FileName].Count;
            }
        }

    }
}
