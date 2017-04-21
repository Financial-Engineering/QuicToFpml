using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quic.FPML;

namespace Quic.Templates
{
    public static class Utilities
    {

        // TODO: kind of expensive to do it this way, just compare the legs directly
        public static bool IsXccySwap(InterestRateStream[] streams)
        {
            var set = new HashSet<string>();

            foreach (var notional in streams.Select(stream => stream.calculationPeriodAmount.Item)
                .OfType<Calculation>().Select(calculation => calculation.Item).OfType<Notional>())
            {
                set.Add(notional.notionalStepSchedule.currency.Value);
            }

            return set.Count > 1; // If there are 2 or more unique currencies return true
        }

        public static bool IsFloat(InterestRateStream stream)
        {
            return stream.resetDates != null;
        }

        public static bool IsFixed(InterestRateStream stream)
        {
            return stream.resetDates == null;
        }

        public static string ToQuicDateFormat(DateTime dt)
        {
            return dt.ToString("yyyy/MM/dd");
        }

        public static string YieldCurve(string ccy, bool basisFlag = false)
        {
            return String.Format("{0}{1}.Yield.{0}", ccy, basisFlag ? "_ccy" : String.Empty);
        }

        public static string ExchangeCurve(string ccy)
        {
            return String.Format("{0}.Exchange.USD", ccy);
        }

        public static string ImpliedVolCurve(string ccy1, string ccy2)
        {
            return String.Format("{0}.ImpliedVol.{1}", ccy1, ccy2);
        }

        /// <summary>
        /// Converts FpML business centers to QuIC holiday calendars
        /// </summary>
        /// <param name="cals">An enumeration of calendar strings</param>
        /// <returns>Enumeration of converted calendars based on BusinessCenterDictionary</returns>
        public static IEnumerable<string> ConvertCalendars(IEnumerable<string> cals)
        {
            return cals != null 
                ? cals.Select(cal => StaticConfig.BusinessCenterDictionary.ContainsKey(cal) ? StaticConfig.BusinessCenterDictionary[cal] : cal) 
                : null;
        }
    }
}
