using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Quic.CSV
{
    // TODO: Create a new base class "SimpleCsvFile" that is not dependent of document structure
    /// <summary>
    /// Generic CSV files as defined by QuIC
    /// </summary>
    public abstract class CsvFile : IDisposable
    {

        protected uint KeyIndex { get; set; }
        protected string FileName { get; set; }
        protected bool InMemory { get; set; }

        // TODO: Replace with System.Runtime.Caching.MemoryCache
        protected static ConcurrentDictionary<string, DocumentStructure> CacheDictionary { get; set; }
        protected static ConcurrentDictionary<string, FileSystemWatcher> Watcher { get; set; }

        // Used to store temporary items needed by RunBatch
        protected Dictionary<string, string> TemporaryItems;

        static CsvFile()
        {
            CacheDictionary = new ConcurrentDictionary<string, DocumentStructure>();
            Watcher = new ConcurrentDictionary<string, FileSystemWatcher>();
        }

        protected CsvFile()
        {
            TemporaryItems = new Dictionary<string, string>();
        }

        protected CsvFile(string fname, uint index = 0)
            : this()
        {
            FileName = fname;
            KeyIndex = index;
            InMemory = false;
        }

        protected static Dictionary<string, string[]> ToDictionary(IEnumerable<string> lines, uint index)
        {
            return lines
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("//"))
                .GroupBy(s => s.Split(',')[index])
                .Select(group => new { @group.Key, g = @group })
                .ToDictionary(item => item.Key, item => item.g.ToArray());
        }

        public virtual bool ContainsKey(string key)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            return CacheDictionary[FileName].NodeDictionaries[DocumentStructure.FieldKeys.Counterparty].ContainsKey(key) 
                || TemporaryItems.ContainsKey(key);
        }

        public virtual void Add(string key, string item)
        {
            if (!TemporaryItems.ContainsKey(key))
                TemporaryItems.Add(key, item);
        }

        public virtual string[] this[string key]
        {
            get
            {
                if (!CacheDictionary.ContainsKey(FileName))
                    Init();

                var map = CacheDictionary[FileName].NodeDictionaries[DocumentStructure.FieldKeys.Counterparty];
                return map.ContainsKey(key) ? map[key] : (TemporaryItems.ContainsKey(key) ? TemporaryItems[key].Split(',') : new[] { "" });
            }
        }

        // Inefficient but simpler than a multi-level dictionary
        public virtual string GetField(string cpty, string field, DocumentStructure.FieldKeys key)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var index = CacheDictionary[FileName].Fields.Values.FindIndex(s => s == field);

            var info = GetBy(key, cpty);
            return info.Length > 0 && index >= 0 ? info[0].Split(',')[index] : String.Empty;
        }

        public virtual void Init()
        {
            if (CacheDictionary.ContainsKey(FileName)) return;

            InitWatcher();

            var doc =
                File.ReadAllLines(FileName)
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .Where(s => !s.StartsWith("//"));

            var enumerable = doc as IList<string> ?? doc.ToList();
            var structure = new DocumentStructure
            {
                // The following code relies on the fact that I'm NOT resetting the enumerable at each section
                // if one does reset it will still function correctly albeit less efficient
                Fields = new DocumentStructure.Section<List<string>>
                {
                    Name = "Field Names:",
                    CommaSeparated = true,
                    Values = enumerable
                        .SkipWhile(s => !s.StartsWith("Field Names:"))
                        .Skip(1)
                        .TakeWhile(s => !s.StartsWith("Plugins:"))
                        .TakeWhile(s => !s.StartsWith("Nodes:"))
                        .Select(s => s.TrimEnd(',')).ToList()
                },
                Plugins = new DocumentStructure.Section<List<string>>
                {
                    Name = "Plugins:",
                    Values = enumerable
                        .SkipWhile(s => !s.StartsWith("Plugins:"))
                        .Skip(1)
                        .TakeWhile(s => !s.StartsWith("Nodes:")).ToList(),
                },
                Nodes = new DocumentStructure.Section<List<string>>
                {
                    Name = "Nodes:",
                    Values = enumerable
                        .SkipWhile(s => !s.StartsWith("Nodes:"))
                        .Skip(1).ToList()

                }
            };

            CacheDictionary[FileName] = structure;
        }

        protected virtual string[] GetBy(DocumentStructure.FieldKeys key, string id)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            Dictionary<string, string[]> dict;

            if (!CacheDictionary[FileName].NodeDictionaries.TryGetValue(key, out dict))
                return new string[0];

            String[] result;

            return !dict.TryGetValue(id, out result) ? new string[0] : result;
        }

        protected virtual void WriteRecord(StreamWriter writer, IEnumerable<string> record)
        {
            writer.WriteLine(String.Join(",", record));
        }

        protected virtual void WriteRecords(string fname, IEnumerable<string> records, bool append = false)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var doc = CacheDictionary[FileName];

            using (var writer = new StreamWriter(fname, append))
            {
                doc.Fields.WriteSection(writer);
                doc.Plugins.WriteSection(writer);

                if (doc.Nodes.Count > 0)
                    writer.WriteLine("Nodes:");

                foreach (var node in records)
                    writer.WriteLine(node);
            }
        }

        public virtual void WriteCounterparty(string fname, IEnumerable<string> cpty, bool append = false)
        {
            var trades = new List<string>();

            foreach (var doc in cpty)
            {
                trades.AddRange(GetRecordsAsList(doc));
            }

            var list = trades
                .Where(trade => !TemporaryItems.ContainsKey(trade.Split(',')[0]))
                .Concat(TemporaryItems.Values).ToList();

            WriteRecords(fname, list);
        }

        public virtual IEnumerable<string> GetRecordsList(DocumentStructure.FieldKeys key,string cpty)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var list = CacheDictionary[FileName].NodeDictionaries[key].Values
                .Where(s => s[0].Split(',')[1] == cpty)
                .Select(s => s[0]).ToList();

            if (TemporaryItems.ContainsKey(cpty))
                list.Add(TemporaryItems[cpty]);

            return list;
        }

        public Dictionary<string, IEnumerable<string>> GetRecordsMap(params string[] cptys)
        {
            var map = cptys.Where(s => !String.IsNullOrWhiteSpace(s))
                .ToDictionary(cpty => cpty, cpty => GetRecordsList(DocumentStructure.FieldKeys.Counterparty, cpty));
            return map;
        }

        // Flatten into a single list
        public virtual IEnumerable<string> GetRecordsAsList(params string[] cptys)
        {
            var l = GetRecordsMap(cptys).Values.SelectMany(list => list);
            return l;
        }

        protected static void ClearCache()
        {
            CacheDictionary.Clear();

            foreach (var fileSystemWatcher in Watcher)
            {
                fileSystemWatcher.Value.EnableRaisingEvents = false;
                fileSystemWatcher.Value.Dispose();
            }
            Watcher.Clear();
        }

        protected static void ClearCache(string fname)
        {
            if (CacheDictionary.ContainsKey(fname))
            {
                DocumentStructure docStruct;
                CacheDictionary.TryRemove(fname, out docStruct);
            }

            if (!Watcher.ContainsKey(fname)) return;

            Watcher[fname].EnableRaisingEvents = false;
            Watcher[fname].Dispose();

            FileSystemWatcher watcher;
            Watcher.TryRemove(fname, out watcher);
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            ClearCache(e.FullPath);
        }

        protected void InitWatcher()
        {
            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(FileName),
                Filter = Path.GetFileName(FileName)
            };

            watcher.Changed += OnChanged;
            watcher.Renamed += OnChanged;
            watcher.Deleted += OnChanged;

            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.EnableRaisingEvents = true;

            if (!Watcher.ContainsKey(FileName))
                Watcher.TryAdd(FileName, watcher);
        }

        public void Dispose()
        {
            ClearCache(FileName);
        }
    }
}