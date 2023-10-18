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
        const string HelpString =
@"--------------------
BrainSharp by Ben Roach
Usage:
  BrainSharp.exe -e <path> [options]
    Execute a Brainfuck source file.
    -o <path>   Specify output file path
    -d          Debug mode
    -m          Minify first (faster if source contains many comments)
    -a <size>   Specify data array size (default 30000)
  BrainSharp.exe -m <path> [options]
    Minify a Brainfuck source file.
    -o <path>   Specify output file path
--------------------";


        // larger data values than char?
        // source code streaming?
        // step debugger?
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

                FileStream outfile = null;
                StreamWriter output = null;
                if (args.Contains("-o"))
                {
                    int argIdx = Array.IndexOf(args, "-o") + 1;
                    if (argIdx < args.Length)
                    {
                        try
                        {
                            // close these handles later
                            outfile = File.OpenWrite(args[argIdx]);
                            output = new StreamWriter(outfile);
                            settings.OutputStream = output;
                        }
                        catch
                        {
                            Console.WriteLine("Error opening file. Aborting.");
                            return;
                        }
                    }

                    Console.WriteLine(HelpString);
                    return;
                }
                if (args.Contains("-d"))
                {
                    settings.Debug = true;
                }
                if (args.Contains("-m"))
                {
                    using (StringWriter sw = new StringWriter())
                    {
                        Minifier.Minify(source, sw);
                        source = sw.ToString();
                    }
                }
                if (args.Contains("-a"))
                {
                    int argIdx = Array.IndexOf(args, "-a") + 1;
                    if (argIdx < args.Length && int.TryParse(args[argIdx], out int size))
                    {
                        settings.DataArraySize = size;
                    }

                    Console.WriteLine(HelpString);
                    return;
                }
                
                // execute interpreter
                var interpreter = new Interpreter(source, settings);
                interpreter.Execute();

                // close file output handles, if opened
                outfile?.Close();
                output?.Close();
            }
            // minify mode
            else if (args.Length >= 2 && args[0] == "-m")
            {
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
                
                if (args.Contains("-o"))
                {
                    int argIdx = Array.IndexOf(args, "-o") + 1;
                    if (argIdx < args.Length)
                    {
                        try
                        {
                            using (FileStream outfile = File.OpenWrite(args[argIdx]))
                            using (StreamWriter output = new StreamWriter(outfile))
                            {
                                Minifier.Minify(source, output);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Error opening file. Aborting.");
                            return;
                        }
                    }

                    Console.WriteLine(HelpString);
                    return;
                }
                else
                {
                    Minifier.Minify(source, Console.Out);
                }
            }
            // no match, show help
            else
            {
                Console.WriteLine(HelpString);
                return;
            }
        }
    }
}
