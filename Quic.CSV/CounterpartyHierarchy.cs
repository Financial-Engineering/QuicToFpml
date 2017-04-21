using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quic.CSV
{
    public class CounterpartyHierarchy : CsvFile
    {
        public CounterpartyHierarchy(string fname)
            : base(fname)
        {
            KeyIndex = 0;
        }

        public override void Init()
        {
            base.Init();

            var structure = CacheDictionary[FileName];

            // By Counterparty
            structure.NodeDictionaries.Add(DocumentStructure.FieldKeys.Counterparty, structure.Nodes.Values
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("//"))
                .GroupBy(s => s.Split(',')[KeyIndex])
                .Select(group => new { @group.Key, g = @group })
                .ToDictionary(item => item.Key, item => item.g.ToArray()));

            // By Parent
            structure.NodeDictionaries.Add(DocumentStructure.FieldKeys.Parent, structure.Nodes.Values
                .Where(s => !String.IsNullOrWhiteSpace(s))
                .Where(s => !s.StartsWith("//"))
                .GroupBy(s => s.Split(',')[1])
                .Select(group => new { @group.Key, g = @group })
                .ToDictionary(item => item.Key, item => item.g.ToArray()));

        }

        public string GetParent(string cpty)
        {
            var parent = GetField(cpty, "Parent",DocumentStructure.FieldKeys.Counterparty);
            return String.IsNullOrEmpty(parent) ? cpty : parent;
        }

        public IEnumerable<string> GetChildren(string cpty)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var result = new List<string>();

            String[] children;

            Dictionary<string, string[]> dict;

            if (!CacheDictionary[FileName].NodeDictionaries.TryGetValue(DocumentStructure.FieldKeys.Parent, out dict))
                return result;

            if (dict.TryGetValue(cpty, out children))
                result.AddRange(children.Select(child => child.Split(',')[0]));

            return result;
        }

        public string GetByCounterParty(string cpId)
        {
            if (string.IsNullOrEmpty(cpId)) return String.Empty;

            var cp = GetBy(DocumentStructure.FieldKeys.Counterparty, cpId);
            return  cp.Length > 0 ? cp[0] : String.Empty;
        }

        public Boolean IsParent(string cpty)
        {
            return !String.IsNullOrEmpty(GetField(cpty, "Parent",DocumentStructure.FieldKeys.Counterparty));
        }

        public override IEnumerable<string> GetRecordsList(DocumentStructure.FieldKeys keys, string cpty)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var list = new List<string>();

            String[] records;

            if (CacheDictionary[FileName].NodeDictionaries[keys].TryGetValue(cpty, out records))
                list.Add(String.Join(",", records));

            return list;
        }

        public IEnumerable<string> Match(string key)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var list = CacheDictionary[FileName].NodeDictionaries[DocumentStructure.FieldKeys.Counterparty].Keys.Where(s => s.StartsWith(key)).OrderBy(s => s);

            return list;
        }

        public IEnumerable<string> RegexMatch(string key)
        {
            if (!CacheDictionary.ContainsKey(FileName))
                Init();

            var rexp = new Regex(key);
            var list = CacheDictionary[FileName].NodeDictionaries[DocumentStructure.FieldKeys.Counterparty].Keys.Where(s => rexp.IsMatch(s)).OrderBy(s => s);

            return list;
        }
    }
}