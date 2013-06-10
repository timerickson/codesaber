using System.Collections.Generic;
using System.Text.RegularExpressions;

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
