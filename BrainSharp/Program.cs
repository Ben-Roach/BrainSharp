using System;
using System.IO;
using System.Linq;
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
        // current source line
        public int srcLine { get; private set; }
        // current source column
        public int srcCol { get; private set; }
        // BF code (and maybe comments)
        public string instructions { get; private set; }
        // data array size
        public int dataArraySize { get; private set; }
        // data array
        public byte[] dataArray { get; private set; }

        private bool verbose;

        /* Initializes the interpreter. Must give it BF code as a string,
         * and may specify any desired settings. Note that including comments/
         * non-BF instructions is ok, but will slow down execution.
         */
        public Interpreter(string code, int dataArraySize=30000, bool verbose = false)
        {
            this.dp = 0;
            this.ip = 0;
            this.step = 0;
            this.srcLine = 0;
            this.srcCol = 0;
            this.instructions = code;
            this.dataArraySize = dataArraySize;
            this.dataArray = new byte[dataArraySize];
            this.verbose = verbose;
        }

        /* Executes the BF code.
         */
        public void Execute(int steps=0)
        {
            while (ip < instructions.Length && (steps == 0 || step < steps))
            {
                Step();
                if (verbose)
                    Console.WriteLine(FormatStepMsg());
            }
        }

        public void Step()
        {
            // true when a valid instruction is found and executed without error (takes a step)
            bool executed = false;
            while(!executed && ip < instructions.Length)
            {
                switch (instructions[ip])
                {
                    // execute instructions
                    case '>':
                        dp++;
                        if (dp >= dataArraySize)
                            throw new BrainfuckExecutionException(FormatDebugMsg("Data pointer exceeded upper bound of data array"));
                        executed = true;
                        break;
                    case '<':
                        dp--;
                        if (dp < 0)
                            throw new BrainfuckExecutionException(FormatDebugMsg("Data pointer exceeded lower bound of data array"));
                        executed = true;
                        break;
                    case '+':
                        dataArray[dp]++;
                        executed = true;
                        break;
                    case '-':
                        dataArray[dp]--;
                        executed = true;
                        break;
                    case '.':
                        Console.Write((char)dataArray[dp]);
                        executed = true;
                        break;
                    case ',':
                        dataArray[dp] = (byte)Console.ReadKey().KeyChar;
                        executed = true;
                        break;
                    case '[':
                        if (dataArray[dp] == 0)
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
                                throw new BrainfuckExecutionException(FormatDebugMsg("Matching ']' not found for '['"));
                        }
                        executed = true;
                        break;
                    case ']':
                        if (dataArray[dp] != 0)
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
                                throw new BrainfuckExecutionException(FormatDebugMsg("Matching '[' not found for ']'"));
                        }
                        executed = true;
                        break;
                    // skip non-instructions
                    case '\n':
                        srcLine++;
                        srcCol = -1; // incremented before next iteration
                        break;
                    default:
                        break;
                }
                srcCol++;
                ip++;
            }
            step++;
        }

        private string FormatDebugMsg(string message)
        {
            return string.Format("[{0}, {1}]: {2}", srcLine, srcCol, message);
        }

        private string FormatStepMsg()
        {
            int dataPreviewStart = dp < 10 ? 0 : dp - 10;
            int dataPreviewEnd = dataArray.Length < dp + 11 ? dataArray.Length : dp + 10;
            StringBuilder dataPreview = new StringBuilder();
            for (int i = dataPreviewStart; i <= dp; i++)
                dataPreview.Append(dataArray[i].ToString() + " ");
            string arrow = new string(' ', dataPreview.Length + 4) + "^";
            for (int i = dp + 1; i <= dataPreviewEnd; i++)
                dataPreview.Append(dataArray[i].ToString() + " ");
            string instructionPreview = ip < instructions.Length ? instructions[ip].ToString() : "END";
            return string.Format("[{0}, {1}]: Step {2}, Instruction: {3}\nData: {4}\n{5}\n",
                srcLine, srcCol, step, instructionPreview, dataPreview, arrow);
        }
    }

    /* Contains tools for minifying BF code.
     */
    public static class Minifier
    {
        private static readonly char[] allowedChars = new char[] { '<', '>', '+', '-', '.', ',', '[', ']' };
        // Gets a minified version of the given BF code
        public static string Minify(string source)
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

    /* Transpiles BF code to other (more) popular languages.
     */
    public static class Transpiler
    {
        // TODO
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
                bool verbose = false;
                int arraySize = 30000;
                // read from file
                try
                {
                    code = File.ReadAllText(args[1]);
                }
                catch
                {
                    Console.WriteLine("Error reading file. Aborting.");
                    return;
                }
                // check switches
                if (args.Contains("-v"))
                    verbose = true;
                // execute interpreter
                Interpreter interpreter = new Interpreter(code, arraySize, verbose);
                interpreter.Execute();
            }
            // minify mode (-m <filepath>)
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
BrainSharp by Benjamin Roach
Usage:
  BrainSharp.exe -e <filepath> [-v]   Execute a Brainfuck file
    -v   Verbose mode
  BrainSharp.exe -m <filepath>        Minify a Brainfuck file
--------------------");
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
