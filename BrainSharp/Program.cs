using System;
using System.IO;
using System.Linq;

/* BrainSharp
 * By Ben Roach
 * https://github.com/Ben-Roach/BrainSharp
 * https://en.wikipedia.org/wiki/Brainfuck
 */

namespace BrainSharp
{
    public class BrainSharpExecutionException : Exception
    {
        public BrainSharpExecutionException() { }

        public BrainSharpExecutionException(string message) : base(message) { }

        public BrainSharpExecutionException(string message, Exception inner) : base(message, inner) { }
    }


    class Program
    {
        // larger data values than char?
        // source code streaming?
        // transpiler?
        static void Main(string[] args)
        {
            // execute mode
            if (args.Length >= 2 && args[0] == "-e")
            {
                // read from source file
                string source;
                try
                {
                    source = File.ReadAllText(args[1]);
                }
                catch
                {
                    Console.WriteLine("Error reading file. Aborting.");
                    return;
                }

                // command-line options
                var settings = new InterpreterSettings();
                if (args.Contains("-v"))
                {
                    settings.Verbose = true;
                }
                if (args.Contains("-d"))
                {
                    int argIdx = Array.IndexOf(args, "-d");
                    if (argIdx < args.Length && int.TryParse(args[argIdx], out int size))
                    {
                        settings.DataArraySize = size;
                    }
                }
                if (args.Contains("-m"))
                {
                    source = Minifier.Minify(source);
                }
                
                // execute interpreter
                var interpreter = new Interpreter(source, settings);
                interpreter.Execute();
            }
            // minify mode
            else if (args.Length >= 2 && args[0] == "-m")
            {
                string code;
                try
                {
                    code = File.ReadAllText(args[1]);
                }
                catch
                {
                    Console.WriteLine("Error reading file. Aborting.");
                    return;
                }
                Console.WriteLine(Minifier.Minify(code));
            }
            // no match, show help
            else
            {
                Console.WriteLine(@"--------------------
BrainSharp by Ben Roach
Usage:
  BrainSharp.exe -e <filepath> [ -v | -d <SIZE> | -m ]
    Execute a Brainfuck source file.
    -v   Verbose mode
    -d   Specify data array size (default 30000)
    -m   Minify first (faster if source contains many comments)
  BrainSharp.exe -m <filepath>
    Minify a Brainfuck file.
--------------------");
                return;
            }
        }
    }
}
