using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.PricingContext;

namespace Quic.CSV
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
}
