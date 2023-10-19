using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BrainSharp
{
    /* Settings object for BF interpreter.
     */
    public class InterpreterSettings
    {
        public bool Debug { get; set; } = false;
        public TextReader InputStream { get; set; } = Console.In;
        public TextWriter OutputStream { get; set; } = Console.Out;
        public int DataArraySize { get; set; } = 30000;
        public ulong MaxSteps { get; set; } = 0;
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
        public ulong step { get; private set; }
        // current source line
        public int srcLine { get; private set; }
        // current source column
        public int srcCol { get; private set; }
        // BF code (and maybe comments)
        public string instructions { get; private set; }
        // data array
        public byte[] dataArray { get; private set; }

        // settings object
        private InterpreterSettings settings;

        /* Initializes the interpreter. Must give it BF instructions as a string,
         * and may specify any desired settings. Note that including comments/
         * non-BF instructions is ok, but will slow down execution.
         */
        public Interpreter(string instructions, InterpreterSettings settings)
        {
            this.dp = 0;
            this.ip = 0;
            this.step = 0;
            this.srcLine = 0;
            this.srcCol = 0;
            this.instructions = instructions;
            this.settings = settings;

            this.dataArray = new byte[settings.DataArraySize];
        }


        /* Executes the BF code.
         */
        public void Execute()
        {
            while (ip < instructions.Length && (settings.MaxSteps == 0 || step < settings.MaxSteps))
            {
                while (Step())
                {
                    if (settings.Debug)
                    {
                        settings.OutputStream.WriteLine(FormatStepMsg());
                    }
                }
            }
        }


        public bool Step()
        {
            if (ip < instructions.Length)
            {
                switch (instructions[ip])
                {
                    // execute instructions
                    case '>':
                        dp++;
                        if (dp >= dataArray.Length)
                            throw new BrainSharpExecutionException(FormatDebugMsg("Data pointer exceeded upper bound of data array"));
                        break;

                    case '<':
                        dp--;
                        if (dp < 0)
                            throw new BrainSharpExecutionException(FormatDebugMsg("Data pointer exceeded lower bound of data array"));
                        break;

                    case '+':
                        dataArray[dp]++;
                        break;

                    case '-':
                        dataArray[dp]--;
                        break;

                    case '.':
                        settings.OutputStream.Write((char)dataArray[dp]);
                        break;

                    case ',':
                        dataArray[dp] = (byte)settings.InputStream.Read();
                        break;

                    case '[':
                        if (dataArray[dp] == 0)
                        {
                            // number of ]s to skip, (increment on [, decrement on ])
                            int skip = 0;
                            bool found = false;
                            for (int i = ip + 1; i < instructions.Length; i++)
                            {
                                if (instructions[i] == '[')
                                {
                                    skip++;
                                }
                                else if (instructions[i] == ']' && skip > 0)
                                {
                                    skip--;
                                }
                                else if (instructions[i] == ']' && skip == 0)
                                {
                                    ip = i;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                throw new BrainSharpExecutionException(FormatDebugMsg("Matching ']' not found for '['"));
                            }
                        }

                        break;

                    case ']':
                        if (dataArray[dp] != 0)
                        {
                            // number of [s to skip, (increment on ], decrement on [)
                            int skip = 0;
                            bool found = false;
                            for (int i = ip - 1; i >= 0; i--)
                            {
                                if (instructions[i] == ']')
                                {
                                    skip++;
                                }
                                else if (instructions[i] == '[' && skip > 0)
                                {
                                    skip--;
                                }
                                else if (instructions[i] == '[' && skip == 0)
                                {
                                    ip = i;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                throw new BrainSharpExecutionException(FormatDebugMsg("Matching '[' not found for ']'"));
                            }
                        }

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
                step++;
                return true;
            }
            else
            {
                return false;
            }
        }


        private string FormatDebugMsg(string message)
        {
            return $"[{srcLine}, {srcCol}]: {message}";
        }


        private string FormatStepMsg()
        {
            int width = 10;
            StringBuilder dataPreview = new StringBuilder();
            int dataPreviewStart = dp < width ? 0 : dp - width;
            int dataPreviewEnd = dataArray.Length < dp + width + 1 ? dataArray.Length : dp + width;
            for (int i = dataPreviewStart; i <= dp; i++)
            {
                dataPreview.Append(dataArray[i].ToString() + " ");
            }

            string dataArrow = new string(' ', dataPreview.Length) + "^";
            for (int i = dp + 1; i <= dataPreviewEnd; i++)
            {
                dataPreview.Append(dataArray[i].ToString() + " ");
            }

            StringBuilder instrPreview = new StringBuilder();
            int instrPreviewStart = ip < width ? 0 : ip - width;
            int instrPreviewEnd = instructions.Length < ip + width + 1 ? instructions.Length : ip + width;
            for (int i = instrPreviewStart; i <= ip; i++)
            {
                instrPreview.Append(instructions[i].ToString() + " ");
            }

            string instrArrow = new string(' ', instrPreview.Length) + "^";
            for (int i = ip + 1; i <= instrPreviewEnd; i++)
            {
                instrPreview.Append(instructions[i].ToString() + " ");
            }

            return $"[{srcLine}, {srcCol}]: Step {step}\nInstructions:\n{instrPreview}\n{instrArrow}\nData:\n{dataPreview}\n{dataArrow}\n";
        }
    }
}
