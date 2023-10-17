using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrainSharpUnitTest
{
    [TestClass]
    public class UnitTest
    {
        string TestSourcesPath = "../../TestSources/";


        [TestMethod]
        public void TestHelloWorld()
        {
            var testCases = new List<string>()
            {
                Path.Combine(TestSourcesPath, $"helloworld1.bf"),
                Path.Combine(TestSourcesPath, $"helloworld2.bf"),
                Path.Combine(TestSourcesPath, $"helloworld3.bf")
            };

            foreach (string testCase in testCases)
            {
                using (var istream = new StringReader(""))
                using (var ostream = new StringWriter())
                {
                    string source = File.ReadAllText(testCase);
                    var settings = new BrainSharp.InterpreterSettings();
                    settings.InputStream = istream;
                    settings.OutputStream = ostream;
                    var interpreter = new BrainSharp.Interpreter(source, settings);
                    interpreter.Execute();
                    Assert.AreEqual("Hello World!\n", settings.OutputStream.ToString(), $"Input: {testCase} Output: {settings.OutputStream}");
                }
            }
        }


        [TestMethod]
        public void TestRot13()
        {
            var testCases = new List<string>()
            {
                "Hello World!"
            };

            foreach (string testCase in testCases)
            {
                using (var istream = new StringReader(testCase))
                using (var ostream = new StringWriter())
                {
                    string source = File.ReadAllText(Path.Combine(TestSourcesPath, "rot13.bf"));
                    var settings = new BrainSharp.InterpreterSettings();
                    settings.InputStream = istream;
                    settings.OutputStream = ostream;
                    var interpreter = new BrainSharp.Interpreter(source, settings);
                    interpreter.Execute();
                    Assert.AreEqual("Uryyb Jbeyq!", settings.OutputStream.ToString(), $"Input: {testCase} Output: {settings.OutputStream}");
                }
            }
        }
    }
}
