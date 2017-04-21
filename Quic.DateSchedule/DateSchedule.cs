using System;
using System.Collections;
using System.Collections.Generic;

namespace Quic.DateSchedule
{
    public enum PeriodEnum
    {
        D,
        W,
        F,
        M,
        Q,
        S,
        Y
    }

    public enum RollConventionEnum
    {
        SUN = 0,
        MON = 1,
        TUE = 2,
        WED = 3,
        THU = 4,
        FRI = 5,
        SAT = 6,
        EOM,
        FRN,
        IMM,
        IMMCAD,
        SFE,
        TBILL,
        NONE
    }

    public enum StubEnum
    {
        NONE,
        ShortInitial,
        ShortFinal,
        LongInitial,
        LongFinal
    }

    public class Period
    {
        public Period(BusinessDate startDate, BusinessDate endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public BusinessDate StartDate { get; set; }
        public BusinessDate EndDate { get; set; }
    }

    public class DateSchedule : IEnumerable<Period>
    {
        public DateSchedule()
        {
            StartDate = new BusinessDate();
            EndDate = new BusinessDate();
            Frequency = new PeriodEnum();
            Multiplier = 1;
            Roll = RollConventionEnum.NONE;
            Stub = StubEnum.NONE;
            Calendar = new string[0];
            Dates = new List<BusinessDate>();
            Periods = new List<Period>();
        }

        public String[] Calendar { get; set; }

        public DateSchedule(BusinessDate startDate, BusinessDate endDate,
            PeriodEnum freq, int multiplier = 1, RollConventionEnum roll = RollConventionEnum.NONE, StubEnum stub = StubEnum.NONE, params string[] cal)
        {
            Create(startDate, endDate, freq, multiplier, roll, stub, cal);
        }

        public BusinessDate StartDate { get; set; }
        public BusinessDate EndDate { get; set; }
        public int Multiplier { get; set; }
        public PeriodEnum Frequency { get; set; }
        public RollConventionEnum Roll { get; set; }
        public StubEnum Stub { get; set; }

        public List<BusinessDate> Dates { get; set; }

        public List<Period> Periods { get; set; }

        public BusinessDate AdjustEndDate(BusinessDate bd)
        {
            var rd = new BusinessDate(bd);

            switch (Roll)
            {
                case RollConventionEnum.EOM:
                    rd.Date = bd.Date.Last();
                    break;
                case RollConventionEnum.FRN:
                    rd.Convention = BusinessDayConventionEnum.MODFOLLOWING;
                    rd = rd.AdjustDate();
                    break;
                case RollConventionEnum.IMM:
                    rd.Date = bd.Find(3, DayOfWeek.Wednesday);
                    break;
                case RollConventionEnum.IMMCAD:
                    rd.Date = new BusinessDate(bd.Find(3, DayOfWeek.Monday), new Calendar("GBP", "CAD"));
                    break;
                case RollConventionEnum.SFE:
                    rd.Date = bd.Find(2, DayOfWeek.Friday);
                    break;
                case RollConventionEnum.TBILL:
                    rd = new BusinessDate(bd.Next(DayOfWeek.Monday), new Calendar("USD"));
                    break;
                case RollConventionEnum.SUN:
                case RollConventionEnum.MON:
                case RollConventionEnum.TUE:
                case RollConventionEnum.WED:
                case RollConventionEnum.THU:
                case RollConventionEnum.FRI:
                case RollConventionEnum.SAT:
                    rd.Date = bd.Date.Next((DayOfWeek) Roll);
                    break;
            }

            return rd;
        }

        public void Create(BusinessDate startDate, BusinessDate endDate,
            PeriodEnum freq, int multiplier = 1,
            RollConventionEnum roll = RollConventionEnum.NONE, StubEnum stub = StubEnum.NONE,
            string[] cal = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            Multiplier = multiplier;
            Frequency = freq;
            Roll = roll;
            Stub = stub;
            Calendar = cal;

            Dates = new List<BusinessDate>();
            Periods = new List<Period>();
            Create();
        }

        private void Create()
        {
            var duration = Multiplier;

            var sign = 1;
            Del compare = (a, b) => a < b;
            var ndt = new BusinessDate(StartDate, Calendar);
            var pdt = new BusinessDate(StartDate, Calendar);
            var beginDate = StartDate;
            var endDate = EndDate;

            if ((Stub == StubEnum.ShortInitial || Stub == StubEnum.LongInitial))
            {
                sign = -1;
                compare = (a, b) => a > b;
                ndt = new BusinessDate(EndDate);
                pdt = new BusinessDate(EndDate);
                beginDate = EndDate;
                endDate = StartDate;
            }

            int n = 1;

            Dates.Add(ndt);
            while (compare(ndt, endDate))
            {
                switch (Frequency)
                {
                    case PeriodEnum.D:
                        ndt = beginDate.AddDays(sign * duration * n++);
                        break;
                    case PeriodEnum.W:
                        duration = 7;
                        goto case PeriodEnum.D;
                    case PeriodEnum.F:
                        duration = 14;
                        goto case PeriodEnum.D;
                    case PeriodEnum.M:
                        ndt = beginDate.AddMonths(sign * duration * n++);
                        break;
                    case PeriodEnum.Q:
                        duration = 3;
                        goto case PeriodEnum.M;
                    case PeriodEnum.S:
                        duration = 6;
                        goto case PeriodEnum.M;
                    case PeriodEnum.Y:
                        ndt = beginDate.AddYears(sign * duration * n++);
                        break;
                }

                Dates.Add(ndt);
                Periods.Add(new Period(pdt, ndt));
                pdt = ndt;
            }

            switch (Stub)
            {
                case StubEnum.LongInitial:
                case StubEnum.ShortInitial:
                    Dates[0] = StartDate;
                    break;
                case StubEnum.LongFinal:
                case StubEnum.ShortFinal:
                    Dates[Dates.Count - 1] = EndDate;
                    break;
            }
        }

        private delegate bool Del(BusinessDate a, BusinessDate b);

        public IEnumerator<Period> GetEnumerator()
        {
            return Periods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}