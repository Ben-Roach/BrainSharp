using System;
using System.Text;
using System.Collections.Generic;

namespace BrainSharp
{
    /* Contains tools for minifying BF code.
     */
    public static class Minifier
    {
        private static readonly HashSet<char> allowedChars = new HashSet<char> { '<', '>', '+', '-', '.', ',', '[', ']' };


        /* Generates a minified version of the given BF code.
         */
        public static string Minify(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (allowedChars.Contains(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
