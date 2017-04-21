using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Quic.DTO
{
    [DataContract]
    public abstract class TradeCsv
    {
        [DataMember]
        public DateTime ValueDate { get; set; }

        [DataMember]
        public string[] Measures { get; set; }

        [DataMember]
        public List<Trade> Trades { get; set; }

        [DataMember]
        public bool StandAlone { get; set; }

        [DataContract]
        public class Trade
        {
            [DataMember]
            public Boolean NewTrade { get; set; }            
            [DataMember]
            public Boolean IgnoreTrade { get; set; }
            [DataMember]
            public string TransactionCsv { get; set; }
            [DataMember]
            public string SchedulesCsv { get; set; }
            [DataMember]
            public string CptyHierarchyCsv { get; set; }
            [DataMember]
            public string LegalDocumentCsv { get; set; }
            [DataMember]
            public string TransactionId { get; set; }
            [DataMember]
            public Boolean IsValid
            {
                get
                {
                    return !(String.IsNullOrEmpty(TransactionCsv) && String.IsNullOrEmpty(TransactionId));
                }
                set
                {
                    
                }
            }
        }

        protected TradeCsv()
        {
            StandAlone = false;
        }
    }

    [DataContract]
    public class TradeCsvResponse : TradeResponse
    {
    }
}