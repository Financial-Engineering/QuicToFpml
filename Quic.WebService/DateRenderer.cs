using System;
using System.Globalization;
using Antlr4.StringTemplate;

namespace Quic.WebService
{
    // Used by StringTemplate engine to properly format dates on output
    public class DateRenderer : IAttributeRenderer
    {
        public string ToString(object o, string formatString, CultureInfo culture)
        {
            return ((DateTime) o).ToString("yyyy/MM/dd"); // fixed to QuIC format
        }
    }
}