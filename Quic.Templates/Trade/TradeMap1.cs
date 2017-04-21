using System;

namespace Quic.Templates
{
    public abstract partial class TradeMap
    {
        public string strName { get; set; }

        protected TradeMap()
        {           
        }

        protected TradeMap(string name)
        {
            strName = name;
        }

        public string GenerateTransaction()
        {
            return TransformText();
        }

        public virtual string GenerateSchedule()
        {
            return String.Empty;
        }
    }
}
