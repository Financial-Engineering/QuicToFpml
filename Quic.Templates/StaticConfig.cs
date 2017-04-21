using System.Collections.Generic;
using Quic.FPML;

namespace Quic.Templates
{
    public static class StaticConfig
    {
        public static Dictionary<BusinessDayConventionEnum, string> BdcDictionary = new Dictionary<BusinessDayConventionEnum, string>
        {
            {BusinessDayConventionEnum.FOLLOWING, "AFTER"},
            {BusinessDayConventionEnum.MODFOLLOWING, "MODFOLLOWING"},
            {BusinessDayConventionEnum.PRECEDING, "BEFORE"}, 
            {BusinessDayConventionEnum.MODPRECEDING, "MODPRECEDING"},
        };

        public static Dictionary<string, string> DayCountDictionary = new Dictionary<string, string>
        {
            {"ACT/ACT.ICMA","ACTACT"},
            {"ACT/ACT.ISDA","ACTACT"},
            {"ACT/ACT.ISMA","ACTACT"},
            {"ACT/365L","ACT365"},
            {"ACT/365.FIXED","ACT365FIXED"},
            {"ACT/ACT.AFB","ACTFEB29"},
            {"ACT/360","ACT360"},
            {"BUS/252","BUS252"},
            {"30/360","THIRTY360"},
            {"30E/360","EURO30360"},
            {"30E/360.ISDA","EURO30360"},
        };

        /// <summary>
        /// Map to translate FpML business centers to QuIC calendars
        /// </summary>
        public static Dictionary<string, string> BusinessCenterDictionary = new Dictionary<string, string>
        {
            {"AUSY","AUD"},
            {"AUME","AUD"},
            {"AUBR","AUD"},
            {"AUPE","AUD"},
            {"USNY","USD"},
            {"NYFD","USD"},
            {"JPTO","JPY"},
            {"GBLO","GBP"},
            {"AEAD","AED"},
            {"NZAU","NZD"},
            {"NZWE","NZD"},
            {"SGSI","SGD"},
            {"SEST","SEK"},
            {"HKHK","HKD"},
            {"NOOS","NOK"},
            {"DKCO","DKK"},
            {"EUTA","EUR"},
            {"CHZU","CHF"},
            {"CNSH","CHN"},
        };
    }
}
