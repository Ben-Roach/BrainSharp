using System;
using System.IO;
using BrainSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrainSharpUnitTest
{
    [TestClass]
    public class UnitTest
    {
        const string TestSourcesPath = "../../TestSources";


        [TestMethod]
        [DataRow("helloworld1.bf", "", "Hello World!\n")]
        [DataRow("helloworld1_min.bf", "", "Hello World!\n")]
        [DataRow("helloworld2.bf", "", "Hello World!\n")]
        //[DataRow("helloworld3.bf", "", "Hello World!\n")]
        [DataRow("rot13.bf", "Hello World!", "Uryyb Jbeyq!")]
        [DataRow("rot13_min.bf", "Hello World!", "Uryyb Jbeyq!")]
        [DataRow("rot13.bf", "The quick brown fox jumps over 13 lazy dogs.", "Gur dhvpx oebja sbk whzcf bire 13 ynml qbtf.")]
        public void TestInterpreterExecuteSuccess(string sourceFile, string inputString, string expectedOutput)
        {
            try
            {
                using (var istream = new StringReader(inputString))
                using (var ostream = new StringWriter())
                {
                    string source = File.ReadAllText(Path.Combine(TestSourcesPath, sourceFile));
                    var settings = new InterpreterSettings();
                    settings.InputStream = istream;
                    settings.OutputStream = ostream;
                    var interpreter = new Interpreter(source, settings);

                    interpreter.Execute();

                    Assert.AreEqual(expectedOutput, ostream.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Exception while running test case {sourceFile}, {inputString}:", e);
            }
        }


        [TestMethod]
        [DataRow("<", "BrainSharp.BrainSharpExecutionException")]
        //[DataRow("+[>]", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("[", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("+]", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("[[][][][][][][][][][][][]", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("[[[[[[[[[[[[[]]]]]]]]]]]]", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("[][][][][][][][][][][][]+]", "BrainSharp.BrainSharpExecutionException")]
        [DataRow("[[[[[[[[[[[[]]]]]]]]]]]]+]", "BrainSharp.BrainSharpExecutionException")]
        public void TestInterpreterExecuteFail(string source, string expectedException)
        {
            try
            {
                using (var ostream = new StringWriter())
                {
                    var settings = new InterpreterSettings();
                    settings.OutputStream = ostream;
                    var interpreter = new Interpreter(source, settings);

                    try
                    {
                        interpreter.Execute();

                        Assert.Fail($"No exception thrown");
                    }
                    catch (Exception e)
                    {
                        Assert.AreEqual(expectedException, e.GetType().ToString(), e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Wrong or no exception thrown by test case \"{source}\":", e);
            }
        }


        [TestMethod]
        [DataRow("helloworld1.bf", "helloworld1_min.bf")]
        [DataRow("rot13.bf", "rot13_min.bf")]
        public void TestMinifierExecuteSuccess(string sourceFile, string expectedOutputFile)
        {
            try
            {
                using (var ostream = new StringWriter())
                {
                    string source = File.ReadAllText(Path.Combine(TestSourcesPath, sourceFile));
                    string expectedOutput = File.ReadAllText(Path.Combine(TestSourcesPath, expectedOutputFile));

                    Minifier.Execute(source, ostream);

                    Assert.AreEqual(expectedOutput, ostream.ToString());
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Exception while running test case {sourceFile}:", e);
            }
        }
    }
}
