using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quic.CSV
{

    public class Observables : Schedules 
    {
        public static Dictionary<string, List<string>> ObservablesByCurrency = new Dictionary<string, List<string>>();
 
        public class ObservableMap : LegMap
        {
            public Dictionary<string, string> FieldMap { get; set; }

            public ObservableMap()
            {
                FieldMap = new Dictionary<string, string>();
            }
        }

        public Observables()
        {
        }

        public Observables(string fname)
            : this()
        {
            FileName = fname;
        }

        public override void Init()
        {
            if (CacheDictionary.ContainsKey(FileName)) return;

            InitWatcher();

            CacheDictionary.Add(FileName, new Dictionary<string, LegMap>());

            var lines =
                File.ReadAllLines(FileName).Where(s => s != String.Empty).Where(s => !s.StartsWith("//"));

            var startOfRecord = false;
            var legName = "";

            // TODO: figure out how to thread this... Also make this a proper Mealy statemachine
            foreach (var str in lines.Select(line => new string(line.Where(c => !char.IsControl(c)).ToArray())))
            {
                if (str.EndsWith("}"))
                {
                    startOfRecord = false;
                }

                if (startOfRecord)
                {

                    if (!CacheDictionary[FileName].ContainsKey(legName))
                        CacheDictionary[FileName].Add(legName, new ObservableMap{ Name = legName });

                    if (str != String.Empty)
                    {
                        CacheDictionary[FileName][legName].Fields.Add(str);

                        var fields = str.Split(',').Select(s=>s.Trim()).ToArray();

                        if (fields.Length > 2)
                        {
                            var observableMap = CacheDictionary[FileName][legName] as ObservableMap;
                            if (observableMap != null && !observableMap.FieldMap.ContainsKey(fields[0]))
                                observableMap.FieldMap.Add(fields[0], fields[2]);

                            if (fields[0] == "strCurrency")
                            {
                                if (!ObservablesByCurrency.ContainsKey(fields[2]))
                                    ObservablesByCurrency.Add(fields[2], new List<string>());
                                else
                                {
                                    ObservablesByCurrency[fields[2]].Add(legName);
                                }
                            }
                        }
                    }
                }

                if (str.EndsWith("{"))
                {
                    legName = str.Substring(0, str.IndexOf(',')).Trim();
                    startOfRecord = true;
                }
            }
        }

        public virtual IEnumerable<string> GetAll()
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return CacheDictionary[FileName].Keys;
        }

        public virtual string GetByName(string name)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return CacheDictionary[FileName].ContainsKey(name)
                ? CacheDictionary[FileName][name].ToString()
                : String.Empty;
        }

        public virtual string[] GetByCurrency(string ccy)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return ObservablesByCurrency.ContainsKey(ccy) 
                ? ObservablesByCurrency[ccy].ToArray() 
                : new string[0];
        }
    }
}