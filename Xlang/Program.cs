using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using Xlang.SharpDetection;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Xlang.SharpDetectionObjects;

namespace Xlang
{
    class Program
    {
        


        static void Main(string[] args)
        {
            //Extract JSON information
            extractJsonSharp();
            extractJsonPy();

            //Analyze Python calls inside sharp classes and alter jsons.
            analyzePythonCallsInCSharp();

            //Generate .h headers
            extractHeaders();

        }

        static void analyzePythonCallsInCSharp()
        {
            var sharpFiles = Directory.GetFiles(@"C:\Users\benas\Desktop\lang\Xlang\src\Sharp", "*.*", SearchOption.AllDirectories)
           .Where(s => s.EndsWith(".txt"));

            SharpAnalyzer sharpAnalyzer = new SharpAnalyzer();
            foreach (String path in sharpFiles)
            {
                sharpAnalyzer.inspectPythonCalls(path);
            }
        }

        static void extractJsonPy()
        {
            var pyFiles = Directory.GetFiles(@"C:\Users\benas\Desktop\lang\Xlang\src\Python", "*.*", SearchOption.AllDirectories)
           .Where(s => s.EndsWith(".py"));

            PyAnalyzer pyAnalizer = new PyAnalyzer();
            foreach (String path in pyFiles)
            {
                pyAnalizer.PyClassAnalyzer(path);
            }
        }


        static void extractJsonSharp()
        {
            var sharpFiles = Directory.GetFiles(@"C:\Users\benas\Desktop\lang\Xlang\src\", "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".txt"));

            SharpAnalyzer sharpAnalyzer = new SharpAnalyzer();
            foreach (String path in sharpFiles)
            {
                sharpAnalyzer.CSharpClassAnalyzer(path);
            }
        }

        static void extractHeaders()
        {

            var sharpFiles = Directory.GetFiles(@"C:\Users\benas\Desktop\lang\Xlang\out\", "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".json"));

            HeaderGenerator sharpHeaderGenerator = new HeaderGenerator();
            
            
            foreach (String path in sharpFiles)
            {
                Console.WriteLine(sharpHeaderGenerator.GenerateHeader(path));
            }
        }



        static String compareFiles(String fileOne, String fileTwo)
        {
            var md5 = MD5.Create();
            
            var streamOne = File.OpenRead(fileOne);
            var streamTwo = File.OpenRead(fileTwo);
            
            var sumOne = Convert.ToBase64String(md5.ComputeHash(streamOne));
            var sumTwo = Convert.ToBase64String(md5.ComputeHash(streamTwo));

            return sumTwo.Equals(sumOne) ? "Files match" : "Files do not match";
        }

    }

}