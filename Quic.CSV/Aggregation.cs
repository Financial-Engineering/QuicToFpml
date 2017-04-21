using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quic.CSV
{
    /// <summary>
    /// Represents the Aggregation.csv format
    /// </summary>
    public class Aggregation : CsvFile
    {
        private static new Dictionary<string, List<string>> CacheDictionary { get; set; }

        static Aggregation()
        {
            CacheDictionary = new Dictionary<string, List<string>>();
        }

        public Aggregation(string fname):this()
        {
            FileName = fname;
        }

        private Aggregation()
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
                var list = File.ReadAllLines(FileName)
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .Where(s => ! s.StartsWith("//"))
                    .SkipWhile(s => s.StartsWith("Keys:"))
                    .TakeWhile(s => !s.StartsWith("Aggregation:"))
                    .Select(s => s.TrimEnd(',')).ToList();

                CacheDictionary[FileName] = list;
            }
        }

        /// <summary>
        /// Index of aggregation field as defined in Keys section
        /// </summary>
        /// <param name="field">As defined in the Keys section</param>
        /// <returns>Location of key field, -1 if not found</returns>
        public int FindIndex(string field)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();
            
            return CacheDictionary[FileName].FindIndex(s => s == field);
        }

        public List<string> GetFields()
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return CacheDictionary[FileName];
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

        /// <summary>
        /// Remove the aggregation fields from a transaction string
        /// </summary>
        /// <param name="transaction">Full transaction string</param>
        /// <returns>String with transaction fields removed</returns>
        public string RemoveFields(string transaction)
        {
            var y = transaction.Split(',');
            return string.Join(",", y.Skip(Count + 2).Take(y.Length - Count + 2));
        }

        /// <summary>
        /// Remove the aggregation fields from transaction field array
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>Array with transaction fields removed</returns>
        public string[] RemoveFields(string[] transaction)
        {
            return transaction.Skip(Count + 2).Take(transaction.Length - Count + 2).ToArray();
        }
    }
}
