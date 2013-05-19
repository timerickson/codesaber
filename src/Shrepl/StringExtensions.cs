using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeSaber.Shrepl
{
    public static class StringExtensions
    {
        public static IEnumerable<string> GetLines(this string raw)
        {
            return Regex.Split(raw, @"\r\n|\r|\n");
        }
    }
}
