using System;
using System.IO;
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
        public static void Minify(string source, TextWriter output)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (allowedChars.Contains(c))
                    sb.Append(c);
            }
            output.Write(sb.ToString());
        }
    }
}
