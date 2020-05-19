using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xlang.SharpDetection;

namespace Xlang.SharpDetectionObjects
{
    class PyAnalyzer
    {

        static Regex variablesRegex = new Regex(@"([a-zA-Z]+)[ ]*=[ ]*([a-zA-Z\d ""]+)");
        static Regex methodsRegex = new Regex(@"(def [a-zA-Z_]+)\(([a-zA-Z, ]+)\):");

        public void PyClassAnalyzer(String path)
        {
            String pyClass = File.ReadAllText(path);
            
            ClassInfo classObject = extractClass(Path.GetFileNameWithoutExtension(path));
            List<PropertyInfo> variables = extractVariables(pyClass);
            List<MethodInfo> methods = extractMethods(pyClass);
            
            String lang = "Py";


            FormedObject pyObject = new FormedObject(classObject, lang, null, variables, methods, null);
            var jsonString = JsonConvert.SerializeObject(pyObject);
            File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + Path.GetFileNameWithoutExtension(path) + ".json", jsonString);
            Console.WriteLine(jsonString);
        }

        List<PropertyInfo> extractVariables(String text)
        {
            List<PropertyInfo> variables = new List<PropertyInfo>();
            foreach (Match m in variablesRegex.Matches(text))
            {

                string name = m.Groups[1].Value;
                string type = m.Groups[2].Value.Contains('"') ? "String" : "double";
                string accessModifier = "?";
                bool isStatic = false;
                variables.Add(new PropertyInfo(name, type, accessModifier, isStatic));
            }
            return variables;
        }

        List<MethodInfo> extractMethods(String text)
        {
            List<MethodInfo> methodsObjects = new List<MethodInfo>();
            foreach (Match m in methodsRegex.Matches(text))
            {
                Console.WriteLine(m.Value);
                
                String[] methodAnnotation = Regex.Split(m.Groups[1].Value, "[ ]+", RegexOptions.IgnoreCase);
                String[] methodParameters = Regex.Split(m.Groups[2].Value, ",[ ]+", RegexOptions.IgnoreCase);

                List<ParamInfo> parameters = extractParams(methodParameters);

                String acessModifier = methodAnnotation[0];
                String name = methodAnnotation[1];

                String returnType = "";
                bool isStatic = false;

                methodsObjects.Add(new MethodInfo(isStatic, name, returnType, acessModifier, parameters));
               
    }
            return methodsObjects;
        }

        private List<ParamInfo> extractParams(String[] parameters)
        {
            List<ParamInfo> allParams = new List<ParamInfo>();
            foreach (String parameter in parameters)
            {
                    allParams.Add(new ParamInfo("?", parameter, ""));
            }
            return allParams;
        }

        ClassInfo extractClass(String fileName)
        {
            return new ClassInfo(fileName, "private", false);
        }

    }
}
