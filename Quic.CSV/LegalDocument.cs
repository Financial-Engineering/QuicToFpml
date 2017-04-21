using System;
using System.Collections.Generic;
using System.Linq;

namespace Quic.CSV
{
    public class LegalDocument : CsvFile
    {
        public LegalDocument(string fname) : base(fname)
        {
            KeyIndex = 0;
        }

        public override void Init()
        {
            base.Init();

            var structure = CacheDictionary[FileName];

            // By Legal Document
            structure.NodeDictionaries.Add(DocumentStructure.FieldKeys.LegalDocument, 
                ToDictionary(structure.Nodes.Values, 0));

            // By Counterparty
            structure.NodeDictionaries.Add(DocumentStructure.FieldKeys.Counterparty, 
                ToDictionary(structure.Nodes.Values, 1));
        }

        public virtual string GetFieldByCounterparty(string cpty, string field)
        {
           return base.GetField(cpty, field, DocumentStructure.FieldKeys.Counterparty); 
        }

        public IEnumerable<string> GetByCounterParty(string cpId)
        {
            return GetBy(DocumentStructure.FieldKeys.Counterparty, cpId).Select(s => s.Split(',')[0]);
        }

        public IEnumerable<string> GetByCounterParties(IEnumerable<string> cptys)
        {
            return cptys.SelectMany(GetByCounterParty).Distinct();
        }

        public string GetFieldByLegalDocument(string name, string field)
        {
            return base.GetField(name, field,DocumentStructure.FieldKeys.LegalDocument);
        }


        public string GetByLegalDocument(string legalDocId)
        {
            var result = GetBy(DocumentStructure.FieldKeys.LegalDocument, legalDocId);

            return result.Length > 0 ? result[0] : String.Empty;
        }

    }
}