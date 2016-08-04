using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextTtransformer.Model;

namespace UnitTestTextTransformer
{
    [TestClass]
    public class TestProcessor
    {
        String testText = "Maximka very good programmer, every \r\n day I am make beautiful; code.\r\n";
        String actual = "Maximka shy programmer, every \r\n day I am make beautiful; code.\r\n";
        String sourcePath = "unitTest.txt";
        String destPath = "unitTest.res.txt";

        [TestMethod]
        public void GeneralTestProcessor()
        {
            String result;
            StreamWriter writer= File.CreateText(sourcePath);
            writer.Write(testText);
            writer.Close();
            TextProcessor processor = new TextProcessor(sourcePath,destPath );
            processor.Process(line=>Regex.Replace(line,"very good","shy"));

            result = File.ReadAllText(destPath);
            Assert.AreEqual(result, actual);
        }
    }
}
