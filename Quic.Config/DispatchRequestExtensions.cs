using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Quic.FPML;
using Quic.Config.Schemas;

namespace Quic.Config
{
    public static class DispatchRequestExtensions
    {
        /// <summary>
        /// Convert dispatch result to a key value pair map
        /// </summary>
        /// <param name="result">Dispatch result obtained from running a dispatch request</param>
        /// <returns>Map of key/value pairs</returns>
        public static Dictionary<string, List<string>> ToDictionary(this DispatchResult result)
        {
            var map = result.Variant.Item as Map;

            if (map != null)
            {
                return map.MapEntry.ToDictionary(entry => entry.Key, entry =>
                {
                    if (entry.Variant.Item.GetType().Name == "String")
                        return new List<string>{entry.Variant.Item.ToString().TrimEnd('\n')};

                    if (entry.Variant.Item.GetType().Name == "Vector")
                    {
                        var vect = entry.Variant.Item as Vector;
                        if (vect != null) return vect.Variant.Select(v =>
                        {
                            // TODO: Add additional cases for other types
                            if (v.Item.GetType() == typeof(Schemas.Double))
                            {
                                var d1 = v.Item as Schemas.Double;
                                return (d1 != null ? d1.Value : 0).ToString(CultureInfo.InvariantCulture);
                            }
                            return ((DateTime)v.Item).ToString("yyyy-MM-dd");

                        }).ToList();
                    }

                    var d = entry.Variant.Item as Schemas.Double;
                    return new List<string>{d != null ? d.Value.ToString(CultureInfo.InvariantCulture) : "0"};
                });
            }

            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Convert a dispatch result object to an FpML ValuationReport
        /// </summary>
        /// <param name="result">Dispatch result obtained from running a dispatch request</param>
        /// <returns>FpML string with ValuationReport as root element</returns>
        public static string ToFpml(this DispatchResult result)
        {
            var report = new ValuationReport
            {
                header = new NotificationMessageHeader
                {
                    messageId = new MessageId(),
                    sentBy = new MessageAddress()
                },
                tradeValuationItem = new[]
                {
                    new TradeValuationItem
                    {
                        valuationSet = new ValuationSet
                        {
                            assetValuation = new[]
                            {
                                new AssetValuation
                                {
                                    quote = ((Map) result.Variant.Item).MapEntry.Select(item => new Quotation
                                    {
                                        measureType = new AssetMeasureType
                                        {
                                            Value = item.Key
                                        },
                                        value = (decimal) ((Schemas.Double) item.Variant.Item).Value
                                    }).ToArray()
                                }
                            }
                        }
                    }
                }
            };

            return Utilities.Serialize(report);
        }
    }
}
