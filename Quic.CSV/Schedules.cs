using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Quic.Config;

namespace Quic.CSV
{
    public class Schedules : CsvFile
    {
        public class LegMap
        {
            public Schedules Schedules { get; set; }

            public LegMap()
            {
                Fields = new List<string>();
                SubLegs = new List<string>();
                CompoundingSchedule = new List<string>();
            }

            public LegMap(Schedules sched):this()
            {
                Schedules = sched;
            }

            public string Name { get; set; }
            public List<string> Fields { get; set; }
            public List<string> SubLegs { get; set; }
            public List<string> CompoundingSchedule { get; set; }

            public override string ToString()
            {
                // If there are sublegs, i.e. multi-leg then recursively resolve
                var str = SubLegs.Aggregate("", (current, subLeg) => current + Schedules[subLeg]);

                str += Name + ",{" + Environment.NewLine;
                str = Fields.Aggregate(str, (current, s) => current + (s + Environment.NewLine));
                str += "}" + Environment.NewLine;
                return CompoundingSchedule.Aggregate(str, (current, s) => current + (s + Environment.NewLine));
            }
        }

        public bool StandAlone { get; set; }

        /// <summary>
        ///     Cache with primary key of file location
        /// </summary>
        protected new static Dictionary<string, Dictionary<string, LegMap>> CacheDictionary
        {
            get;
            set;
        }

        static Schedules()
        {
            CacheDictionary = new Dictionary<string, Dictionary<string, LegMap>>();
        }

        public Schedules()
        {
        }

        public Schedules(string fname, bool standAlone = false)
            : this()
        {
            FileName = fname;
            StandAlone = standAlone;
        }

        public override void Init()
        {
            if (!CacheDictionary.ContainsKey(FileName))
            {
                InitWatcher();

                CacheDictionary.Add(FileName, new Dictionary<string, LegMap>());

                var lines =
                    File.ReadAllLines(FileName).Where(s => s != String.Empty).Where(s => !s.StartsWith("//"));

                var startOfLegMap = false;
                var legName = "";

                // Strip any control characters from line
                foreach (var str in lines.Select(line => new string(line.Where(c => !char.IsControl(c)).ToArray())))
                {
                    if (str.EndsWith("}"))
                    {
                        startOfLegMap = false;
                    }

                    if (startOfLegMap)
                    {
                        if (!CacheDictionary[FileName].ContainsKey(legName))
                            CacheDictionary[FileName].Add(legName, new LegMap{Schedules = this, Name = legName});

                        if (str != String.Empty)
                        {
                            CacheDictionary[FileName][legName].Fields.Add(str);

                            var fields = str.Split(',');
                            if (fields[0] == "ampLeg" || fields[0] == "mpCompoundingSchedule") // If there is a ampLeg field then this is a multi-leg map
                                 CacheDictionary[FileName][legName].SubLegs = fields.Skip(2).Take(fields.Length - 2).Select(s => s.Trim()).ToList();
                        }
                    }

                    if (str.EndsWith("{"))
                    {
                        legName = str.Substring(0, str.IndexOf(',')).Trim();
                        startOfLegMap = true;
                    }

                    // if we are not inside a legmap then this is a compounding schedule
                    if (!(str.EndsWith("}") || startOfLegMap))
                    {
                        if (CacheDictionary[FileName].ContainsKey(legName))
                            CacheDictionary[FileName][legName].CompoundingSchedule.Add(str);
                    }
                }
            }
        }

        public new LegMap this[string key]
        {
            get
            {
                if (!CacheDictionary.ContainsKey(FileName))
                    Init();

                return !CacheDictionary[FileName].ContainsKey(key) ? new LegMap(this) : CacheDictionary[FileName][key];
            }
        }

        public virtual void WriteRecord(StreamWriter writer, string leg, string[] record)
        {
            writer.WriteLine(leg + ",{");
            foreach (var s in record)
            {
                writer.WriteLine(s);
            }
            writer.WriteLine("}");
        }


        public virtual void WriteByLegalDocument(string fname, IEnumerable<string> legalDoc, bool append = false)
        {
            var context = ServiceConfig.Context;

            var port = new Portfolio(context.Portfolio);
            var aggSize = new Aggregation(context.Aggregation).Count;

            using (var writer = new StreamWriter(fname, append))
            {

                if (!StandAlone)
                {
                    foreach (var legs in legalDoc.SelectMany(doc => (from trade in port.GetByLegalDocument(doc)
                        select trade.Split(',')
                        into fields
                        let legmap = ServiceConfig.Products[fields[1]].LegMapOffsets
                        where legmap != null
                        select legmap.Select(leg => fields[leg + aggSize + 2]).ToArray())))
                    {
                        WriteLegMap(writer, legs);
                    }
                }
                // don't care if duplicate or not
                foreach (var temporaryItem in TemporaryItems)
                {
                    writer.WriteLine(temporaryItem.Value);
                }
            }
        }

        public override void WriteCounterparty(string fname, IEnumerable<string> cpty, bool append = false)
        {
            var context = ServiceConfig.Context;

            var port = new Portfolio(context.Portfolio);
            var aggSize = new Aggregation(context.Aggregation).Count;

            using (var writer = new StreamWriter(fname, append))
            {
                if (!StandAlone)
                {
                    foreach (var legs in cpty.SelectMany(s =>
                        (from trade in port[s]
                            select trade.Split(',')
                            into fields
                            let legmap = ServiceConfig.Products[fields[1]].LegMapOffsets
                            where legmap != null
                            select legmap.Select(leg => fields[leg + aggSize + 2]).ToArray())))
                    {
                        WriteLegMap(writer, legs);
                    }
                }

                // don't care if duplicate or not
                foreach (var temporaryItem in TemporaryItems)
                {
                    writer.WriteLine(temporaryItem.Value);
                }
            }
        }

        
        public virtual void WriteLegMap(StreamWriter writer, params string[] legs)
        {
            if (legs == null) 
                return;

            foreach (var leg in legs.Where(leg => !String.IsNullOrEmpty(leg)))
                    writer.WriteLine(this[leg]);

        }
    }
}