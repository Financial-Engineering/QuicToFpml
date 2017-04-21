using System;

namespace Quic.Dispatch
{
    internal static class Extensions
    {
        public static string ToString(this Boolean b)
        {
            return b ? "true" : "false";
        }
    }
}