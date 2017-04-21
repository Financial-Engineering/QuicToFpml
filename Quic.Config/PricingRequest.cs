using System;

namespace Quic.Config
{
    public enum BatchModeEnum
    {
        MTM,
        CVA
    }

    public class PricingRequest
    {
        public string Product { get; set; }
        public string[] Measures { get; set; }
        public PricingContext Context { get; set; }
        public Date ValueDate { get; set; }
        public string PricingTransaction { get; set; }
        public string Transaction { get; set; }
        public string Schedules { get; set; }
        public string CptyHierarchy { get; set; }
        public string LegalDocument { get; set; }
        //public Trade FpmlTradeDetails { get; set; }
        public Boolean NewTrade { get; set; }
        public string Cpty { get; set; }
        public string TradeId { get; set; }
        public double PV { get; set; }
        public double CVA { get; set; }
        public double FVA { get; set; }
        public BatchModeEnum BatchMode { get; set; }
    }
}