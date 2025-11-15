using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GetDigits(this string input)
        {
            var result = String.IsNullOrEmpty(input) ? "0" : new String(input.Where(Char.IsDigit).ToArray());
            return string.IsNullOrEmpty(result) ? "0" : result;
        }
    }
}
