using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quic.CSV
{
    /// <summary>
    /// Represents the Aggregation.csv format
    /// </summary>
    public class SimParams : CsvFile
    {
        private static new Dictionary<string, Dictionary<string,string>> CacheDictionary { get; set; }

        static SimParams()
        {
            CacheDictionary = new Dictionary<string, Dictionary<string, string>>();
        }

        public SimParams(string fname):this()
        {
            FileName = fname;
        }

        private SimParams()
        {
        }

        /// <summary>
        /// Loads cache dictionary with aggregation hierarchy
        /// </summary>
        public override void Init()
        {
            if (!CacheDictionary.ContainsKey(FileName))
            {
                InitWatcher();

                // read fields between Keys: and Aggregation:
                var dict = File.ReadAllLines(FileName)
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .Where(s => ! s.StartsWith("//"))
                    .TakeWhile(s => !s.StartsWith("Date"))
                    .Select(s => s.Split(',')).ToDictionary(item => item[0], item => item[1]);

                CacheDictionary[FileName] = dict;
            }
        }

        public new string this[string key]
        {
            get
            {
                if (!CacheDictionary.ContainsKey(FileName))
                    Init();

                string value;

                return CacheDictionary[FileName].TryGetValue(key, out value) ? value : String.Empty;
            }
        }
    }
}
