namespace Quic.CSV
{
    public class LegalDocumentRecord : CsvRecord
    {
        public string strLegalDocumentId { get; set; }
        public string CounterpartyID { get; set; }
        public string ParentDocId { get; set; }
        public string eResultType { get; set; }
        public bool bNetting { get; set; }
        public bool bCollateralize { get; set; }
        public double rUserThreshold { get; set; }
        public double rUserMinimumTransfer { get; set; }
        public double rCounterpartyThreshold { get; set; }
        public double rCounterpartyMinimumTransfer { get; set; }
        public int nUserLag { get; set; }
        public int nCounterpartyLag { get; set; }
        public double rInitialCollateral { get; set; }
        public string dtMonitoringStart_unused { get; set; }
        public int nMonitoringInterval_unused { get; set; }
        public double rCounterpartyIndependentFixed { get; set; }
        public double rCounterpartyIndependentCoeff_unused_zero { get; set; }
        public string strCounterpartyFXCurveName_Agreement { get; set; }
        public double rUserIndependentFixed { get; set; }
        public double rUserIndependentCoeff_unused_zero { get; set; }
        public string strUserFXCurveName_unused { get; set; }
        public string strPostsCollateral { get; set; }
        public double rUserUpfrontMargin { get; set; }
        public double rCounterpartyUpfrontMargin { get; set; }
        public bool bUseScheduleParams { get; set; }
        public bool bRounding { get; set; }
        public string strUserRDir { get; set; }
        public double rUserRAmt { get; set; }
        public string strCptyRDir { get; set; }
        public double rCptyRAmt { get; set; }
        public string strHolidaySet { get; set; }
        public string strCSAFreq { get; set; }
        public string kCallDay { get; set; }
        public string strCalldayRoll { get; set; }
        public int nCallMonthFirst { get; set; }
        public int nCallMonthFreq { get; set; }
        public string strMonitoringDateList { get; set; }
        public bool bCallOnSimDate { get; set; }

    }
}