using System;
using System.IO;
using System.Text;

/* Brainsharp
 * By Benjamin Roach
 * 
 */

namespace Brainsharp
{
    public class BrainFuckExecutionException : Exception
    {
        public BrainFuckExecutionException() { }

        public BrainFuckExecutionException(string message) : base(message) { }

        public BrainFuckExecutionException(string message, Exception inner) : base(message, inner) { }
    }

    public class Interpreter
    {
        public int dp { get; private set; } // data pointer
        public int ip { get; private set; } // instruction pointer
        public byte[] data_array { get; private set; } // data array
        public int src_line { get; private set; } // current source line
        public int src_col { get; private set; } // current source column

        public Interpreter(int array_size=30000)
        {
            this.dp = 0;
            this.ip = 0;
            this.src_line = 0;
            this.src_col = 0;
            this.data_array = new byte[array_size];
        }

        public void Execute(string instructions)
        {
            while (ip < instructions.Length)
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
                                else if (instructions[i] == '[' && skip > 0)
                                    skip--;
                                else if (instructions[i] == ']' && skip == 0)
                                {
                                    ip = i;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                throw new BrainFuckExecutionException(FormatDebugLine("Matching ']' not found for '['"));
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
                                throw new BrainFuckExecutionException(FormatDebugLine("Matching '[' not found for ']'"));
                        }
                        break;
                    // track line number
                    case '\n':
                        src_line++;
                        src_col = -1;
                        break;
                    default:
                        break;
                }
                src_col++;
                ip++;
            }
        }

        private string FormatDebugLine(string message)
        {
            return String.Format("[{0}, {1}]: {2}", src_line, src_col, message);
        }
    }

    public class Minifier
    {
        private readonly char[] allowedChars = new char[] { '<', '>', '+', '-', '.', ',', '[', ']' };
        public string Minify(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if (Array.IndexOf(allowedChars, c) == -1)
                    sb.Append(c);
            }
            return sb.ToString();
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
        // step-by-step execution?
        // verbose mode?

        // get filename
        string code;
        if (args.Length > 0)
            code = File.ReadAllText(args[0]);
        else
            Console.WriteLine("Enter code to execute:");
        code = Console.ReadLine();

        Brainsharp.Interpreter interpreter = new Brainsharp.Interpreter();
        interpreter.Execute(code);
        Console.ReadLine();
    }
}
