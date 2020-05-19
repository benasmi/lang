using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xlang.SharpDetection;
using Xlang.StructureObjects;

namespace Xlang.SharpDetectionObjects
{
    class SharpAnalyzer
    {


        static Regex pythonUsagesRegex = new Regex(@"%([a-zA-Z_\d]+)%");

        static Regex variablesRegex = new Regex(@"((static )?(string|int|double|float|String)( static)?[ ]+[a-zA-Z]+)[ ]*=");
        static Regex propertiesRegex = new Regex(@".*{([ ]+)?get([ ]+)?;([ ]+)?set([ ]+)?;([ ]+)");

        static Regex methodsRegex = new Regex(@"(static [ ]*)?((public|private)[ ]+)?(static [ ]*)?(void|int|string|double|float)[ ]+[a-zA-Z\d]+\(([a-zA-Z, \[\]]+)?\)");
        static Regex methodBodyRegex = new Regex(@"(static [ ]*)?((public|private)[ ]+)?(static [ ]*)?(void|int|string|double|float)[ ]+[a-zA-Z\d]+\(([a-zA-Z, \[\]]+)?\)[.* \n]*{?[.* \t\r a-zA-Z=#<+>(""! ?);\d\n(({)?]*}", RegexOptions.Multiline);
        static Regex paramsRegex = new Regex(@"\(([a-zA-Z ,\[\] \d""]+)\)"); //Need double checking
        static Regex methodAnotationRegex = new Regex(@"(.*)\(");

        static Regex classRegex = new Regex(@".*class([ ]+)?[a-zA-Z\d]+");
        static String importsRegex = @"using([ ])+[a-zA-Z]+;";
        static String[] accessModifiers = { "public", "private", "internal", "protected" };
        static String staticMod = "static";
        static String[] methodTypes = { "int", "string", "String", "void", "double", "float"};

        public void CSharpClassAnalyzer(String path)
        {
            String sharpClass = File.ReadAllText(path);
            ClassInfo classObject = extractClass(sharpClass);
            List<PropertyInfo> props = extractProperties(sharpClass);
            List<PropertyInfo> variables = extractVariables(sharpClass);
            List<MethodInfo> methods = extractMethods(sharpClass);
            String lang = "Sharp";


            FormedObject sharpObject = new FormedObject(classObject, lang, props, variables, methods, null);
            var jsonString = JsonConvert.SerializeObject(sharpObject);
            File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + Path.GetFileNameWithoutExtension(path) + ".json", jsonString);
            //Console.WriteLine(jsonString);

        }

        public void inspectPythonCalls(String path)
        {
            String sharpClass = File.ReadAllText(path);
            String sharpFileName = Path.GetFileNameWithoutExtension(path);

            List<String> pythonUsages = new List<String>();
            String sharpJsonfile = File.ReadAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + sharpFileName + ".json");
            FormedObject sharpClassObj = JsonConvert.DeserializeObject<FormedObject>(sharpJsonfile);

            foreach (Match m in pythonUsagesRegex.Matches(sharpClass))
            {
                pythonUsages.Add(m.Groups[1].Value);
            }

            foreach(String pyUsage in pythonUsages)
            {
                String pyFile = File.ReadAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + pyUsage + ".json");
                FormedObject pyClass = JsonConvert.DeserializeObject<FormedObject>(pyFile);

                Regex findUsages = new Regex(@"([a-z =]+?)?"+ pyUsage + @"\.([a-zA-Z_"", \d;]+)\([a-zA-Z "", \d ]+\)");
                foreach (Match m in findUsages.Matches(sharpClass))
                {
                    
                    updatePyJson(pyClass, m, sharpClass, sharpClassObj);
                }

                var jsonString = JsonConvert.SerializeObject(pyClass);
                File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + pyUsage + ".json", jsonString);
            }
            
            var sharpClassString = JsonConvert.SerializeObject(sharpClassObj);
            File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + sharpFileName + ".json", sharpClassString);

            alterSharpFile(path);
        }

        public void alterSharpFile(String path)
        {
            String sharpClass = File.ReadAllText(path);
            String sharpFileName = Path.GetFileNameWithoutExtension(path);

            List<String> pythonUsages = new List<String>();
            String sharpJsonfile = File.ReadAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + sharpFileName + ".json");
            FormedObject sharpClassObj = JsonConvert.DeserializeObject<FormedObject>(sharpJsonfile);

            foreach (Match m in pythonUsagesRegex.Matches(sharpClass))
            {
                pythonUsages.Add(m.Groups[1].Value);
            }

            foreach (String pyUsage in pythonUsages)
            {
                String pyFile = File.ReadAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + pyUsage + ".json");
                FormedObject pyClass = JsonConvert.DeserializeObject<FormedObject>(pyFile);

                Regex findUsages = new Regex(@"([a-z =]+?)?" + pyUsage + @"\.([a-zA-Z_"", \d;]+)\([a-zA-Z "", \d ]+\)");
                foreach (Match m in findUsages.Matches(sharpClass))
                {

                   String replacement = alterMethodCall(m, sharpClassObj);

                    sharpClass = sharpClass.Replace(m.Value, replacement);
                    Console.WriteLine("asdasd");
                }

            }

            addMethodAnnotations(sharpClass, sharpClassObj, sharpFileName);
        }

        /**
         * Adds method annotations for C++ backend
         */
        private void addMethodAnnotations(String sharpClass, FormedObject sharpObject, String fileName) 
        {
            StringBuilder methodAnotation = new StringBuilder();
            Regex findClassEntry = new Regex(@"static void Main.*");
            Match m = findClassEntry.Match(sharpClass);

            if (sharpObject.PyUsages.Count > 0) methodAnotation.Append("IntPtr Instance;\n");

            for(int i = 0; i<sharpObject.PyUsages.Count; i++)
            {
                methodAnotation.Append("[MethodImpl(MethodImplOptions.InternalCall)]\n");
                methodAnotation.Append("extern private static "+ 
                    sharpObject.PyUsages[i].Methods.ReturnType 
                    + " __internal_"+
                    sharpObject.PyUsages[i].Methods.Name+"(");

                List<ParamInfo> parameters = sharpObject.PyUsages[i].Methods.Params;

               for(int j = 0; j<parameters.Count; j++)
                {
                    methodAnotation.Append((!parameters[j].Type.ToLower().Equals("string") ? "out " : "  ") + parameters[j].Type + " " + parameters[j].Name);
                    if (j != parameters.Count - 1) methodAnotation.Append(",");
                }
                methodAnotation.Append(");\n\n");
            }
            methodAnotation.Append(m.Value);
            sharpClass = sharpClass.Replace(m.Value, methodAnotation.ToString());
            sharpClass = sharpClass.Replace("using System;", "using System;\nusing System.Runtime.CompilerServices;");


            File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\Sharp\" + fileName + ".cs", sharpClass);
        }

       
        /*
         * 
        IntPtr Instance;

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern private static string __internal_get_output_text(string prefix, out int number);

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern private static void __internal_print_custom_text(string text);
         */

        private String alterMethodCall(Match m, FormedObject sharpClass)
        {
            StringBuilder methodCall = new StringBuilder();
            String methodName = m.Groups[2].Value;

            for(int i = 0; i<sharpClass.PyUsages.Count; i++)
            {
                if (sharpClass.PyUsages[i].Methods.Name.Equals(methodName))
                {
                    List<ParamInfo> parameters = sharpClass.PyUsages[i].Methods.Params;
                    for (int j = 0; j<parameters.Count; j++)
                    {
                        methodCall.Append("" + parameters[j].Type + " __" + parameters[j].Name + " = " + parameters[j].Value+";\n");
                    }
                    if (!sharpClass.PyUsages[i].Methods.ReturnType.Equals("void"))
                    {
                        methodCall.Append(m.Groups[1].Value);
                    }
                    methodCall.Append("__internal_" +sharpClass.PyUsages[i].Methods.Name+"(");
                                      
                    for(int j = 0; j<parameters.Count; j++)
                    {
                        if (parameters[j].Type.Equals("string"))
                        {
                            methodCall.Append("__" + parameters[j].Name);
                        }
                        else
                        {
                            methodCall.Append("out __" + parameters[j].Name);
                        }
                        if (j != parameters.Count - 1)
                        {
                            methodCall.Append(",");
                        }
                    }
                    methodCall.Append(")");
                }
            }


            return methodCall.ToString();
            /**
             * 
            string tekstas = python_file.get_output_text("Skaicius", 50);
	        python_file.print_custom_text(tekstas);
             * 
             */


            /*
             * int __number = 50;
            string tekstas = __internal_get_output_text("Skaicius", out __number);
            __internal_print_custom_text(tekstas);
             */

        }



        public void updatePyJson(FormedObject pyClass, Match m, String sharpClass, FormedObject sharpClassObject)
        {
            String line = m.Value;
            String pyMethodUsage = m.Groups[2].Value;
            for(int i = 0; i<pyClass.Methods.Count; i++)
            {
                if (pyClass.Methods[i].Name.Equals(pyMethodUsage))
                {
                    pyClass.Methods[i].ReturnType = getReturnType(line);
                    String[] parameters = Regex.Split(paramsRegex.Match(m.Value).Groups[1].Value, ",[ ]*", RegexOptions.IgnoreCase);
                    for (int j = 0; j < pyClass.Methods[i].Params.Count; j++)
                    {
                        pyClass.Methods[i].Params[j].Type = getParamType(parameters[j], sharpClass);
                        pyClass.Methods[i].Params[j].Value = parameters[j];
                    }

                    if (sharpClassObject.PyUsages == null)
                    {
                        sharpClassObject.PyUsages = new List<PyUsage>();
                    }
                    sharpClassObject.PyUsages.Add(new PyUsage(pyClass.Methods[i], pyClass.Class.Name));
                }
                
            }
        }

        public string getParamType(String a, String file)
        {
            Regex number = new Regex(@"\d+");
            Regex textual = new Regex(@"""([a - zA - Z]+)""");

            String paramType = "";
            if (number.IsMatch(a))
            {
                paramType = "int";
            }
            if (a.Contains("\""))
            {
                paramType = "string";
            }

            if (!number.IsMatch(a) && paramType.Equals(""))
                paramType = findParamTypeInFile(a, file);

            return paramType;
            
        }


        public String findParamTypeInFile(String paramName, String file)
        {
            Regex paramTypeRegex = new Regex(@"([a-zA-Z]+)[ ]*" + paramName);
            return paramTypeRegex.Match(file).Groups[1].Value;
        }
        
        public String getReturnType(string line)
        {
            Regex returnType = new Regex(@"([a-zA-Z]+)[ ]*[a-zA-Z]+ [ ]*=");
            if (returnType.IsMatch(line))
            {
                return returnType.Match(line).Groups[1].Value;
            }
            else
            {
                return "void";
            }
        }


        List<PropertyInfo> extractVariables(String text)
        {
            List<PropertyInfo> variables = new List<PropertyInfo>();
            String textAltered = methodBodyRegex.Replace(text, "");
            foreach (Match m in variablesRegex.Matches(text))
            {
                string name = "";
                string type = "";
                string accessModifier = "private";
                bool isStatic = false;

                String[] variableRes = Regex.Split(m.Groups[1].Value, "[ ]+", RegexOptions.IgnoreCase);
                foreach (String v in variableRes)
                {
                    if (v.Equals(staticMod))
                    {
                        isStatic = true;
                    }
                    else if (accessModifiers.Contains(v))
                    {
                        accessModifier = v;
                    }
                    else if (methodTypes.Contains(v))
                    {
                        type = v;
                    }
                    else if (name.Equals(""))
                    {
                        name = v;
                    }
                }
                variables.Add(new PropertyInfo(name, type, accessModifier, isStatic));
            }
            return variables;
        }

        List<MethodInfo> extractMethods(String text)
        {
            List<MethodInfo> methodsObjects = new List<MethodInfo>();
            foreach (Match m in methodsRegex.Matches(text))
            {
                String[] methodAnnotation = Regex.Split(methodAnotationRegex.Match(m.Value).Groups[1].Value, "[ ]+", RegexOptions.IgnoreCase);
                List<ParamInfo> parameters = extractParams(m);

                String acessModifier = "private";
                String returnType = "void";
                bool isStatic = false;
                String name = "";

                foreach (String k in methodAnnotation)
                {
                    if (accessModifiers.Contains(k))
                    {
                        acessModifier = k;
                    }
                    else if (methodTypes.Contains(k))
                    {
                        returnType = k;
                    }
                    else if (k.Equals(staticMod))
                    {
                        isStatic = true;
                    }
                    else if (!k.Equals("void"))
                    {
                        name = k;
                    }
                }
                methodsObjects.Add(new MethodInfo(isStatic, name, returnType, acessModifier, parameters));
            }

            return methodsObjects;
        }

        private List<ParamInfo> extractParams(Match m)
        {
            List<ParamInfo> allParams = new List<ParamInfo>();
            String[] parameters = Regex.Split(paramsRegex.Match(m.Value).Groups[1].Value, ",[ ]*", RegexOptions.IgnoreCase);
            foreach (String parameter in parameters)
            {
                String[] individualParams = Regex.Split(parameter, "[ ]+", RegexOptions.IgnoreCase);
                if (individualParams.Length > 1)
                    allParams.Add(new ParamInfo(individualParams[0], individualParams[1], ""));
            }
            return allParams;
        }

        List<PropertyInfo> extractProperties(String text)
        {
            Regex singleProperty = new Regex(@"(private|public)?(static)?[a-zA-Z]+ ");
            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach (Match m in propertiesRegex.Matches(text))
            {
                String name = "";
                String type = "";
                String accessModifier = "private";
                bool isStatic = false;

                String props = m.Value.Split("{")[0];
                String[] propsRes = Regex.Split(props, "[ ]+", RegexOptions.IgnoreCase);
                foreach (String r in propsRes)
                {
                    if (r.Equals(staticMod))
                    {
                        isStatic = true;
                    }
                    else if (accessModifiers.Contains(r))
                    {
                        accessModifier = r;
                    }
                    else if (methodTypes.Contains(r))
                    {
                        type = r;
                    }
                    else if (name.Equals(""))
                    {
                        name = r;
                    }
                }
                properties.Add(new PropertyInfo(name, type, accessModifier, isStatic));
            }
            return properties;
        }


        ClassInfo extractClass(String text)
        {
            String name = "";
            String accessModifier = "private";
            bool isStatic = false;

            String[] result = Regex.Split(classRegex.Match(text).Value, "[ ]+", RegexOptions.IgnoreCase);
            foreach (String r in result)
            {
                if (accessModifiers.Contains(r))
                {
                    accessModifier = r;
                }
                else if (r.Equals(staticMod))
                {
                    isStatic = true;
                }
                else if (!r.Equals("class"))
                {
                    name = r;
                }
            }
            return new ClassInfo(name, accessModifier, isStatic);
        }

    }
}
