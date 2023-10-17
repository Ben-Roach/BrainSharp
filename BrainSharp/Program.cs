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

  BrainSharp.exe -m <filepath>
    Minify a Brainfuck file.
--------------------");
                return;
            }
        }
    }
}

