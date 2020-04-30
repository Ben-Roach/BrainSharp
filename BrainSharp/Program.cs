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
        public string instructions { get; private set; } // instructions
        public int step { get; private set; }

        public Interpreter(string instructions, int array_size=30000)
        {
            this.dp = 0;
            this.ip = 0;
            this.step = 0;
            this.src_line = 0;
            this.src_col = 0;
            this.data_array = new byte[array_size];
            this.instructions = instructions;
        }

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
                    src_col = -1; // incremented before next step
                    break;
                default:
                    break;
            }
            src_col++;
            step++;
            ip++;
        }

        private string FormatDebugLine(string message)
        {
            return string.Format("[{0}, {1}]: {2}", src_line, src_col, message);
        }
    }

    public static class Minifier
    {
        private static readonly char[] allowedChars = new char[] { '<', '>', '+', '-', '.', ',', '[', ']' };
        public static string Minify(string source)
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

        // execute mode (-e <filepath>)
        if (args.Length == 2 && args[0] == "-e")
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
            Brainsharp.Interpreter interpreter = new Brainsharp.Interpreter(code);
            interpreter.Execute();
            Console.ReadLine();
        }
        // no match, show help
        else
        {
            Console.WriteLine(
                @"Usage:
                BrainSharp.exe -e <filepath>    Execute a Brainfuck file");
            return;
        }

        
    }
}
