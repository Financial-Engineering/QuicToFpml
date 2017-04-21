using System;
using System.Xml.Serialization;

namespace Quic.Config
{
    [XmlRoot(Namespace = "nab.com.au", ElementName = "ObservablesMap", DataType = "string", IsNullable = true)]
    public class ObservablesMap : DataMap<ObservablesMap.Key, ObservablesMap.ReferenceIndex>
    {
        [XmlType(TypeName = "ObservablesKey")]
        public class Key : IEquatable<Key>
        {
            public string Ccy { get; set; }
            public string Tenor { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Ccy != null ? Ccy.GetHashCode() : 0) * 397) ^ (Tenor != null ? Tenor.GetHashCode() : 0);
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;

                return obj.GetType() == GetType() && Equals((Key) obj);
            }

            public bool Equals(Key other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Ccy, other.Ccy) && string.Equals(Tenor, other.Tenor);
            }
        }

        public class ReferenceIndex
        {
            public string Ccy { get; set; }
            public string Index { get; set; }
            public string Source { get; set; }
            public string Tenor { get; set; }
            public string Dcc { get; set; }
            public string Instrument { get; set; }

            public override string ToString()
            {
                return String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Ccy, Index, Source, Tenor, Dcc, Instrument);
            }
        }
    }
}