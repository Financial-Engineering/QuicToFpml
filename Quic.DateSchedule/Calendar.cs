using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quic.Config;

namespace Quic.DateSchedule
{

    #region UnknownCurrencyException Class

    public class UnknownCurrencyException : Exception
    {
        public UnknownCurrencyException()
        {
        }

        public UnknownCurrencyException(string message)
            : base(message)
        {
        }

        public UnknownCurrencyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    #endregion

    #region Calendar Class

    public class Calendar : IEnumerable<DateTime>
    {
        private static readonly Dictionary<string, HashSet<DateTime>> Cals = new Dictionary<string, HashSet<DateTime>>();

        static Calendar()
        {
            var entries = File.ReadAllLines(ServiceConfig.Context.HolidaysCsv);
                
            foreach (var fields in entries.Select(row => row.Split(',')))
            {
                if (!Cals.ContainsKey(fields[0]))
                    Cals.Add(fields[0], new HashSet<DateTime>());

                var dts = Cals[fields[0]];

                foreach (var field in fields.Skip(1))
                {
                    dts.Add(DateTime.Parse(field));
                }
            }
        }

        public Calendar()
        {
            Holidays = new HashSet<DateTime>();
        }

        public Calendar(string ccy)
        {
            SetCalendar(ccy);
        }

        public Calendar(params string[] ccys)
        {
            SetCalendar(ccys);
        }

        public Calendar(IEnumerable<string> ccys)
        {
            SetCalendar(ccys);
        }

        private HashSet<DateTime> Holidays { get; set; }

        public bool IsValid(string cal)
        {
            return Cals.ContainsKey(cal);
        }

        public void SetCalendar(string ccy)
        {
            if (!Cals.ContainsKey(ccy))
                throw new UnknownCurrencyException("Unsupported Currency: " + ccy);

            Holidays = Cals[ccy];
        }

        public void SetCalendar(IEnumerable<string> ccys)
        {
            foreach (var ccy in ccys)
            {
                if (!Cals.ContainsKey(ccy))
                    throw new UnknownCurrencyException(ccy);

                if (Holidays == null)
                    Holidays = Cals[ccy];
                else
                    Holidays.UnionWith(Cals[ccy]);
            }
        }

        public void AddCalendar(string ccy)
        {
            if (!Cals.ContainsKey(ccy))
                throw new UnknownCurrencyException(ccy);

            if (Holidays == null)
                Holidays = Cals[ccy];
            else
                Holidays.UnionWith(Cals[ccy]);
        }

        // This function makes no provision for Israel or Islamic countries
        public bool IsWeekend(DateTime dt)
        {
            return (dt.DayOfWeek == DayOfWeek.Saturday) ||
                   (dt.DayOfWeek == DayOfWeek.Sunday);
        }

        public bool IsHoliday(DateTime dt)
        {
            return Holidays != null && Holidays.Contains(dt);
        }

        public bool IsBusinessDay(DateTime dt)
        {
            return !(IsHoliday(dt) || IsWeekend(dt));
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return Holidays.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Holidays.GetEnumerator();
        }
    }

    #endregion
}