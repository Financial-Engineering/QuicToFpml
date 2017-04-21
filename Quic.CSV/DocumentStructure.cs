using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.Config;


namespace Quic.CSV
{
    /// <summary>
    /// Defines the internal structure of a CSV file
    /// </summary>
    public class DocumentStructure
    {
        public class Section<T> where T : IEnumerable<string>, new()
        {
            public string Name { get; set; }
            public bool CommaSeparated { get; set; }

            public T Values { get; set; }

            public Section()
            {
                Name = "";
                CommaSeparated = false;
                Values = new T();
            }

            public Section(Section<T> section)
            {
                Name = section.Name;
                CommaSeparated = section.CommaSeparated;
                Values = section.Values.Copy();
            }

            public int Count
            {
                get
                {
                    return Values.Count();
                }
            }

            public void WriteSection(StreamWriter writer)
            {
                if (Count == 0)
                    return;

                writer.WriteLine(Name);

                if (CommaSeparated)
                {
                    writer.WriteLine(String.Join(",\n", Values));
                }
                else
                {
                    foreach (var value in Values)
                        writer.WriteLine(value);
                }
            }
        }

        public Section<List<string>> Fields { get; set; }
        public Section<List<string>> Plugins { get; set; }
        public Section<List<string>> Nodes { get; set; }

        public enum FieldKeys
        {
            Counterparty,
            Parent,
            LegalDocument,
            TradeId,
            CurveId,
            CurveType,
            Currency
        }

        /// <summary>
        /// Node dictionaries contain CSV nodes ordered by different keys
        /// </summary>
        public Dictionary<FieldKeys, Dictionary<string, string[]>> NodeDictionaries { get; set; }

        public DocumentStructure()
        {
            Fields = new Section<List<string>>();
            Plugins = new Section<List<string>>();
            Nodes = new Section<List<string>>();
            NodeDictionaries = new Dictionary<FieldKeys, Dictionary<string, string[]>>();
        }

        public DocumentStructure(DocumentStructure structure)
        {
            Fields  = new Section<List<string>>(structure.Fields);
            Plugins = new Section<List<string>>(structure.Plugins);
            Nodes   = new Section<List<string>>(structure.Nodes);

            NodeDictionaries = structure.NodeDictionaries.Copy();
        }

        public int Count
        {
            get
            {
                return NodeDictionaries.Values.First().Values.Count;
            }
        }
    }
}