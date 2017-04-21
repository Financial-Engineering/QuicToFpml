using System.Xml.Serialization;

namespace Quic.Config
{
    [XmlRoot(Namespace = "nab.com.au", ElementName = "ProductMap", DataType = "string", IsNullable = true)]
    public class ProductMap : DataMap<string, ProductMap.PricingSupport>
    {
        public enum SupportedMeasuresEnum
        {
            Price,
            SpotDelta,
            SpotRate,
            FwdDelta,
            FwdRate,
            FwdPts,
            PV01,
            PV01Pay,
            PV01Rec,
            DV01,
            BV01,
            Duration,
            Convexity,
            Delta,
            Gamma,
            Vega,
            Theta,
            Rho,
            Phi,
            ImpliedVol,
            DeltaRec,
            DeltaPay,
            CashFlows,
            Accrued,
            Premium,
            DeltaStrike
        };

        [XmlElement(ElementName = "ProductKey")]
        public string Key { get; set; }

        public class PricingSupport
        {
            public string Alias { get; set; }
            public SupportedMeasuresEnum[] SupportedMeasures { get; set; }
            public uint[] LegMapOffsets { get; set; }
        }
    }
}