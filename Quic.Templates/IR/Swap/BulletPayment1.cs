using System;
using System.Collections.Generic;
using System.Globalization;
using Quic.FPML;

namespace Quic.Templates
{
    public partial class BulletPayment
    {

        public string strTradeId { get; set; }

        public string strCurrency { get; set; }
        public string adtPayment { get; set; }
        public string arAmount { get; set; }

        public BulletPayment(string strTradeId)
            : base(strTradeId)
        {
            this.strTradeId = strTradeId;
        }

    }
}