using System;
using System.Collections.Generic;
using System.Linq;

namespace Quic.Templates
{
    public abstract partial class Trade
    {
        public FPML.Trade trade { get; set; }
        public string Ccy { get; set; }
        public List<string> PartyList { get; set; }
        public List<Tuple<string,string>> Parties { get; set; }
        public string strTradeId { get; set; }

        public string strProduct { get; set; }

        private static String GetRandomString(Int32 length)
        {
            var seedBuffer = new Byte[4];
            using (var rngCryptoServiceProvider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(seedBuffer);
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random(BitConverter.ToInt32(seedBuffer, 0));
                return new String(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }

        protected Trade()
        {           
        }

        protected Trade(FPML.Trade trade, IEnumerable<Tuple<string, string>> parties)
        {
            Parties = parties.ToList();
            this.trade = trade;
            Ccy = "USD";
            strTradeId = trade.id ?? GetRandomString(8);

            strProduct = productMap[trade.Item.GetType().Name];
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
