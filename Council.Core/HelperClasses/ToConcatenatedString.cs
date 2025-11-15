using System;
using System.Collections.Generic;

namespace Council.Core.ExtentionMethods
{
    public static class ExtentionMethods
    {
        public static string ToConcatenatedString(
            this List<string> list
            , string separator)
        {
            return String.Join(separator, list);
        }

        public static string ToConcatenatedString(
            this string[] list
            , string separator)
        {
            return String.Join(separator, list);
        }
    }
}