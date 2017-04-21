using System;

namespace Quic.Dispatch
{
    public class Date
    {
        private readonly DateTime _value;

        public Date()
        {
            _value = DateTime.Now;
        }

        public Date(DateTime dt)
        {
            _value = dt;
        }

        public Date(string str)
        {
            _value = DateTime.Parse(str);
        }

        public override string ToString()
        {
            return _value.ToString("yyyy-MM-ddT00:00:00");
        }

        public static explicit operator Date(DateTime dt)
        {
            return new Date(dt);
        }

        public static implicit operator DateTime(Date dt)
        {
            return dt._value;
        }
    }
}