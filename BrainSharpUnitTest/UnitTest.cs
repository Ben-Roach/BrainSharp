using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrainSharpUnitTest
{
    [TestClass]
    public class UnitTest
    {
        const string TestSourcesPath = "../../TestSources";


        [TestMethod]
        [DataRow("helloworld1.bf", "", "Hello World!\n")]
        [DataRow("helloworld2.bf", "", "Hello World!\n")]
        [DataRow("helloworld3.bf", "", "Hello World!\n")]
        [DataRow("rot13.bf", "Hello World!", "Uryyb Jbeyq!")]
        public void TestMethod(string sourceFile, string inputString, string expectedOutput)
        {
            try
            {
                using (var istream = new StringReader(inputString))
                using (var ostream = new StringWriter())
                {
                    string source = File.ReadAllText(Path.Combine(TestSourcesPath, sourceFile));
                    var settings = new BrainSharp.InterpreterSettings();
                    settings.InputStream = istream;
                    settings.OutputStream = ostream;
                    var interpreter = new BrainSharp.Interpreter(source, settings);

                    interpreter.Execute();

                    Assert.AreEqual(expectedOutput, ostream.ToString(),
                        $"Source: {sourceFile}, Input: {inputString}, Expected: {expectedOutput}, Output: {ostream}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Exception while running test case {sourceFile}, {inputString}:", e);
            }
        }
    }
}
