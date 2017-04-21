using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Quic.Config
{
    /// <summary>
    /// Defines a generic map of key/value pairs stored in a list and provided as a dictionary
    /// </summary>
    /// <typeparam name="TKey">Key must be equatable</typeparam>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot(Namespace = "nab.com.au", ElementName = "DataMap", DataType = "string", IsNullable = true)]
    [XmlIncludeAttribute(typeof(ProductMap))]
    [XmlIncludeAttribute(typeof(ObservablesMap))]
    public abstract class DataMap<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public class DataItem
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        public List<DataItem> Items { get; set; }

        [XmlIgnore]
        public Dictionary<TKey, TValue> ItemDict
        {
            get
            {
                return Items.Select(item => new { item.Key, item.Value }).ToDictionary(item => item.Key, item => item.Value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return ItemDict.ContainsKey(key);
        }

       public TValue this[TKey key]
       {
           get
           {
               TValue result;
               if(!ItemDict.TryGetValue(key, out result))
                   throw new KeyNotFoundException(String.Format("item not found {0}", key.ToString()));
               return result;
           }
       }

        public string ToXml()
        {
            return Utilities.Serialize(this);
        }

        public static DataMap<TKey, TValue> FromXml(string xml)
        {
            return Utilities.Deserialize<DataMap<TKey, TValue>>(xml);
        }
    }
}
