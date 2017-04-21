using System;

namespace Quic.DateSchedule
{
    // Business date roll behaviours 
    public enum BusinessDayConventionEnum
    {
        FOLLOWING,
        FRN,
        MODFOLLOWING,
        PRECEDING,
        MODPRECEDING,
        NONE
    }

    // Summary:
    //     Represents a business date with 0..n holiday calendars
    // 
    public class BusinessDate : IComparable<BusinessDate>, IEquatable<BusinessDate>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BusinessDate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _date.GetHashCode();
                hashCode = (hashCode*397) ^ (Calendar != null ? Calendar.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Convention;
                return hashCode;
            }
        }

        //private Calendar cal;
        private DateTime _date;

        public BusinessDate()
        {
            Calendar = new Calendar();
            _date = new DateTime();
            Convention = BusinessDayConventionEnum.NONE;
        }

        public BusinessDate(BusinessDate bd)
        {
            Calendar = bd.Calendar;
            _date = bd._date;
            Convention = bd.Convention;
        }

        public BusinessDate(DateTime dt)
        {
            Create(dt, new Calendar());
        }

        public BusinessDate(DateTime dt, Calendar cal, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(dt, cal, conv);
        }


        public BusinessDate(BusinessDate dt, Calendar cal, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(dt, cal, conv);
        }

        public BusinessDate(DateTime dt, string cal, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(dt, cal, conv);
        }

        public BusinessDate(int year, int month, int day, Calendar cal,
            BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(new DateTime(year, month, day), cal, conv);
        }

        public BusinessDate(int year, int month, int day, string cal, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(new DateTime(year, month, day), cal, conv);
        }

        public BusinessDate(DateTime dt, params string[] cals)
        {
            Create(dt, new Calendar(cals));
        }

        public Calendar Calendar { get; set; }
        public BusinessDayConventionEnum Convention { get; set; }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                _date = AdjustDate().Date;
            }
        }

        public int CompareTo(object value)
        {
            return _date.CompareTo(value);
        }

        public int CompareTo(BusinessDate value)
        {
            return _date.CompareTo(value);
        }

        public bool Equals(BusinessDate value)
        {
            if (ReferenceEquals(null, value)) return false;
            if (ReferenceEquals(this, value)) return true;
            return _date.Equals(value._date) && Equals(Calendar, value.Calendar) && Convention == value.Convention;
        }

        public static implicit operator DateTime(BusinessDate bd)
        {
            return bd.Date;
        }

        public static implicit operator BusinessDate(DateTime dt)
        {
            return new BusinessDate(dt);
        }

        public void Create(DateTime dt, Calendar cal = null, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Calendar = cal;
            _date = dt;
            Convention = conv;

            _date = AdjustDate().Date;
        }

        public void Create(DateTime dt, string cal, BusinessDayConventionEnum conv = BusinessDayConventionEnum.NONE)
        {
            Create(dt, new Calendar(cal), conv);
        }

        private BusinessDate AddBusinessDays(int incr)
        {
            var ndt = new BusinessDate(this);

            while (!Calendar.IsBusinessDay(ndt.Date))
                ndt._date = ndt._date.AddDays(incr);

            return ndt;
        }

        public BusinessDate Add(int multiplier, PeriodEnum period, BusinessDayConventionEnum bdc = BusinessDayConventionEnum.NONE, Calendar cal = null)
        {
            var ndt = new BusinessDate(this, cal, bdc);

            var duration = Math.Abs(multiplier);
            var sign = Math.Sign(multiplier);

            switch (period)
            {
                case PeriodEnum.D:
                    ndt = ndt.AddDays(sign * duration);
                    break;
                case PeriodEnum.W:
                    duration = 7;
                    goto case PeriodEnum.D;
                case PeriodEnum.F:
                    duration = 14;
                    goto case PeriodEnum.D;
                case PeriodEnum.M:
                    ndt = ndt.AddMonths(sign * duration);
                    break;
                case PeriodEnum.Q:
                    duration = 3;
                    goto case PeriodEnum.M;
                case PeriodEnum.S:
                    duration = 6;
                    goto case PeriodEnum.M;
                case PeriodEnum.Y:
                    ndt = ndt.AddYears(sign * duration);
                    break;
            }

            return ndt;
        }

        public BusinessDate AdjustDate()
        {
            var ndt = new BusinessDate(this);

            if (!Calendar.IsBusinessDay(ndt.Date))
            {
                switch (Convention)
                {
                    case BusinessDayConventionEnum.FOLLOWING:
                        ndt = AddBusinessDays(1);
                        break;
                    case BusinessDayConventionEnum.MODFOLLOWING:
                        ndt = AddBusinessDays(1);
                        if (ndt.Date.Month != Date.Month)
                        {
                            goto case BusinessDayConventionEnum.PRECEDING;
                        }
                        break;
                    case BusinessDayConventionEnum.PRECEDING:
                        ndt = AddBusinessDays(-1);
                        break;
                    case BusinessDayConventionEnum.MODPRECEDING:
                        ndt = AddBusinessDays(-1);
                        if (ndt.Date.Month != Date.Month)
                        {
                            goto case BusinessDayConventionEnum.FOLLOWING;
                        }
                        break;
                    default:
                        ndt = AddBusinessDays(1);
                        break;
                }
            }

            return ndt;
        }

        public BusinessDate PrevBusinessDay()
        {
            BusinessDate pdt = AddBusinessDays(-1);

            return pdt;
        }

        public static bool operator ==(BusinessDate d1, BusinessDate d2)
        {
            return d2 != null && (d1 != null && d1._date == d2._date);
        }

        public static bool operator ==(BusinessDate d1, DateTime d2)
        {
            return d1 != null && d1._date == d2;
        }

        public static bool operator ==(DateTime d1, BusinessDate d2)
        {
            return d2 != null && d1 == d2._date;
        }

        public static bool operator !=(BusinessDate d1, BusinessDate d2)
        {
            return d2 != null && (d1 != null && d1._date != d2._date);
        }

        public static bool operator !=(BusinessDate d1, DateTime d2)
        {
            return d1 != null && d1._date != d2;
        }

        public static bool operator !=(DateTime d1, BusinessDate d2)
        {
            return d2 != null && d1 != d2.Date;
        }

        public static bool operator <(BusinessDate d1, BusinessDate d2)
        {
            return d1._date < d2._date;
        }

        public static bool operator <(BusinessDate d1, DateTime d2)
        {
            return d1._date < d2;
        }

        public static bool operator <(DateTime d1, BusinessDate d2)
        {
            return d1 < d2._date;
        }

        public static bool operator <=(BusinessDate d1, BusinessDate d2)
        {
            return d1._date <= d2._date;
        }

        public static bool operator <=(BusinessDate d1, DateTime d2)
        {
            return d1._date <= d2;
        }

        public static bool operator <=(DateTime d1, BusinessDate d2)
        {
            return d1 <= d2._date;
        }

        public static bool operator >(BusinessDate d1, BusinessDate d2)
        {
            return d1._date > d2._date;
        }

        public static bool operator >(BusinessDate d1, DateTime d2)
        {
            return d1._date > d2;
        }

        public static bool operator >(DateTime d1, BusinessDate d2)
        {
            return d1 > d2._date;
        }

        public static bool operator >=(BusinessDate d1, BusinessDate d2)
        {
            return d1._date >= d2._date;
        }

        public static bool operator >=(BusinessDate d1, DateTime d2)
        {
            return d1._date >= d2;
        }

        public static bool operator >=(DateTime d1, BusinessDate d2)
        {
            return d1 >= d2._date;
        }

        public static TimeSpan operator -(BusinessDate d1, BusinessDate d2)
        {
            return d1._date - d2._date;
        }

        public static TimeSpan operator -(BusinessDate d1, DateTime d2)
        {
            return d1._date - d2;
        }

        public static TimeSpan operator -(DateTime d1, BusinessDate d2)
        {
            return d1 - d2._date;
        }

        public static BusinessDate operator +(BusinessDate d1, TimeSpan ts)
        {
            return new BusinessDate(d1._date + ts, d1.Calendar);
        }

        public BusinessDate AddDays(int days)
        {
            return new BusinessDate(Date.AddDays(days), Calendar, Convention);
        }

        public BusinessDate AddMonths(int months)
        {
            return new BusinessDate(Date.AddMonths(months), Calendar, Convention);
        }

        public BusinessDate AddYears(int years)
        {
            return new BusinessDate(Date.AddYears(years), Calendar, Convention);
        }

        public int Subtract(BusinessDate d1, BusinessDate d2)
        {
            return (d1 - d2).Days;
        }

        public int Subtract(BusinessDate d1, DateTime d2)
        {
            return (d1 - d2).Days;
        }

        public int Subtract(DateTime d1, BusinessDate d2)
        {
            return (d1 - d2).Days;
        }

        public string ToString(IFormatProvider provider)
        {
            return _date.ToString(provider);
        }

        public string ToString(string format = "yyyy-MM-dd")
        {
            return _date.ToString(format);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            return _date.ToString(format, provider);
        }

        public int ToExcel()
        {
            return _date.ToExcel();
        }

        public BusinessDate FromExcel(int excelDate)
        {
            return new BusinessDate(_date.FromExcel(excelDate), Calendar);
        }

        public BusinessDate Next(DayOfWeek dayOfWeek)
        {
            return new BusinessDate(_date.Next(dayOfWeek), Calendar);
        }

        public BusinessDate Find(int num, DayOfWeek dayOfWeek)
        {
            return new BusinessDate(_date.Find(num, dayOfWeek), Calendar);
        }
    }
}