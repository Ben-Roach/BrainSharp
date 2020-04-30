using System;
using System.IO;
using System.Text;

/* BrainSharp
 * By Benjamin Roach
 * https://github.com/Ben-Roach/BrainSharp
 */

namespace BrainSharp
{
    public class BrainfuckExecutionException : Exception
    {
        public BrainfuckExecutionException() { }

        public BrainfuckExecutionException(string message) : base(message) { }

        public BrainfuckExecutionException(string message, Exception inner) : base(message, inner) { }
    }

    /* A BF interpreter.
     */
    public class Interpreter
    {
        // data pointer index
        public int dp { get; private set; }
        // instruction pointer index
        public int ip { get; private set; }
        // current number of steps completed
        public int step { get; private set; }
        // data array
        public byte[] data_array { get; private set; }
        // current source line
        public int src_line { get; private set; }
        // BF code (and maybe comments)
        public string instructions { get; private set; }
        // current source column
        public int src_col { get; private set; }

        /* Initializes the interpreter. Must give it BF code as a string,
         * and may specify any desired settings. Note that including comments/
         * non-BF instructions is ok, but will slow down execution.
         */
        public Interpreter(string code, int array_size=30000, bool verbose=false)
        {
            this.dp = 0;
            this.ip = 0;
            this.step = 0;
            this.src_line = 0;
            this.src_col = 0;
            this.instructions = code;
            this.data_array = new byte[array_size];
        }

        /* Executes the BF code.
         */
        public void Execute(int steps=0)
        {
            while (ip < instructions.Length && (steps == 0 || step < steps))
            {
                Step();
            }
        }

        public void Step()
        {
            switch (instructions[ip])
            {
                // execute instructions
                case '>':
                    dp++;
                    break;
                case '<':
                    dp--;
                    break;
                case '+':
                    data_array[dp]++;
                    break;
                case '-':
                    data_array[dp]--;
                    break;
                case '.':
                    Console.Write((char)data_array[dp]);
                    break;
                case ',':
                    data_array[dp] = (byte)Console.ReadKey().KeyChar;
                    break;
                case '[':
                    if (data_array[dp] == 0)
                    {
                        int skip = 0; // increment with [, decrement with ]
                        bool found = false;

                        for (int i = ip + 1; i < instructions.Length; i++)
                        {
                            if (instructions[i] == '[')
                                skip++;
                            else if (instructions[i] == ']' && skip > 0)
                                skip--;
                            else if (instructions[i] == ']' && skip == 0)
                            {
                                ip = i;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            throw new BrainfuckExecutionException(FormatDebugLine("Matching ']' not found for '['"));
                    }
                    break;
                case ']':
                    if (data_array[dp] != 0)
                    {
                        int skip = 0; // increment with ], decrement with [
                        bool found = false;

                        for (int i = ip - 1; i >= 0; i--)
                        {
                            if (instructions[i] == ']')
                                skip++;
                            else if (instructions[i] == '[' && skip > 0)
                                skip--;
                            else if (instructions[i] == '[' && skip == 0)
                            {
                                ip = i;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            throw new BrainfuckExecutionException(FormatDebugLine("Matching '[' not found for ']'"));
                    }
                    break;
                // update line number
                case '\n':
                    src_line++;
                    src_col = -1; // incremented before next step
                    break;
                default:
                    break;
            }
            // update column number, step, and instruction pointer
            src_col++;
            step++;
            ip++;
        }

        private string FormatDebugLine(string message)
        {
            return string.Format("[{0}, {1}]: {2}", src_line, src_col, message);
        }
    }

    /* Contains tools for minifying BF code.
     */
    public static class Minifier
    {
        private static readonly char[] allowedChars = new char[] { '<', '>', '+', '-', '.', ',', '[', ']' };
        // Gets a minified version of the given BF code
        public static string GetMinified(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (Array.IndexOf(allowedChars, c) != -1)
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }

    // Provides a command-line inteface for BrainSharp.
    public static class BrainSharpCLI
    {
        /* execute a command that is an array of arguments.
         */
        public static void Execute(string[] args)
        {
            // execute mode (-e <filepath>)
            if (args.Length >= 2 && args[0] == "-e")
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
                Interpreter interpreter = new Interpreter(code);
                interpreter.Execute();
                Console.ReadLine();
            }
            // minify mode (-m <filepath>)
            if (args.Length >= 2 && args[0] == "-m")
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
                Console.WriteLine(Minifier.GetMinified(code));
            }
            // no match, show help
            else
            {
                Console.WriteLine(@"Usage:
  BrainSharp.exe -e <filepath>    Execute a Brainfuck file
  BrainSharp.exe -m <filepath>    Minify a Brainfuck file");
                return;
            }
        }

        /* Execute a command that is a single string.
         */
        public static void Execute(string command)
        {
            Execute(command.Split(' '));
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // larger data values than char?
        // allow flexible array length?
        // debug with line, col numbers
        // verbose mode?
        BrainSharp.BrainSharpCLI.Execute(args);
    }
}
