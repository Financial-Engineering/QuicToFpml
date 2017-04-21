namespace Quic.CSV
{
    public class CounterpartyRecord : CsvRecord
    {
        public string Counterparty { get; set; }
        public string Parent { get; set; }
        public bool BranchFlag { get; set; }
        public string pqCreditSpread { get; set; }
        public string pqYieldCurve { get; set; }
        public double rRecovery { get; set; }
        public string pHolidays { get; set; }
        public string g1dtIMMDates { get; set; }
        public string strBootstrapCleanOrDirty { get; set; }
        public string oSolverPrefs { get; set; }
        public int WID { get; set; }
        public string long_name { get; set; }
        public string country_code { get; set; }
        public string country_of_risk { get; set; }
        public string parent_wid { get; set; }
        public string parent_long_name { get; set; }
        public int rating_code { get; set; }
        public string pqHistoricalCurve { get; set; }
        public string pqHistoricalYield { get; set; }
        public string pqGenericCurve { get; set; }
        public string pqGenericYield { get; set; }
        public string mpUserInfo { get; set; }
        public string mpUserGenericInfo { get; set; }
        public string pFXInfo { get; set; }
        public string pqAccCurve { get; set; }
        public string pqAccYield { get; set; }

    }
}