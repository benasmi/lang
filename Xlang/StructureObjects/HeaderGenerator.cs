using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xlang.SharpDetection;


/*
 * 
 * 
 * 
 * 
 


    class python_file : public PythonObject
{
public:
	python_file()
		: PythonObject("python_file")
	{}

	virtual std::string getInstanceID() const override
	{
		return "python_file";
	}
protected:
	// Inherited via ScriptObject
	virtual void resolve(PythonBuilder script) override
	{
		get_output_text = script.resolveMethod<std::string>("get_output_text");
		print_custom_text = script.resolveMethod<void>("print_custom_text");
	}
public:	
	Memory::reference<Method<std::string>> get_output_text;
	Memory::reference<Method<void>> print_custom_text;
}
 */


namespace Xlang.SharpDetectionObjects
{
    class HeaderGenerator
    {

        public String GenerateHeader(String path)
        {
            String json = File.ReadAllText(path);
            FormedObject obj = JsonConvert.DeserializeObject<FormedObject>(json);

            String classHeader = generateClassHeader(obj);
            String resolveBody = generateResolves(obj);
            String generateRefs = generateReferences(obj);

            String gentext = String.Format("" +
                "{0}" +
                "{1}" +
                "{2}", classHeader, resolveBody, generateRefs);


            File.WriteAllText(@"C:\Users\benas\Desktop\lang\Xlang\out\" + Path.GetFileNameWithoutExtension(path) + ".h", gentext);

            return gentext;
        }

        String generateReferences(FormedObject obj)
        {

            StringBuilder variables = VariablesResult(obj, true);
            StringBuilder methods = MethodsResult(obj, true);
            StringBuilder properties = PropertiesResult(obj, true);
            string gentext = String.Format("public:\n\n" +
                "{0}\n\n" +
                "{1}\n\n" +
                "{2}\n\n" +
                "}};", variables.ToString(), methods.ToString(), properties.ToString());

            return gentext;
        }

        String generateClassHeader(FormedObject obj)
        {
            String targetClass = obj.Class.Name;
            String scriptObject = obj.Lang.Equals("Sharp") ? "ScriptObject" : "PythonObject";

            String instanceId = generateInstanceId(obj);
            String glueLanguage = "sharp_glue";
            String glueName = "SharpGlue";

            string gentext = obj.Lang.Equals("Sharp") ?
                
                String.Format("class {0} : public {1}\n" +
               "{{\n" +
               "public:\n" +
               "   {0}() : {1}(\"{2}\",\"{3}\",\"{0}\"){{}}\n{4}"
              , targetClass, scriptObject, glueLanguage, glueName, instanceId) : 
              
              String.Format("class {0} : public {1}\n" +
               "{{\n" +
               "public:\n" +
               "   {0}() : {1}(\"{0}\"){{}}\n{2}"
              , targetClass, scriptObject, instanceId);

            return gentext;
        }

        String generateResolves(FormedObject obj)
        {
            StringBuilder variables = VariablesResult(obj);
            StringBuilder methods = MethodsResult(obj);
            StringBuilder properties = PropertiesResult(obj);
            String pythonCalls = obj.Lang.Equals("Sharp") ? generatePythonMethodCalls(obj) : "";
            String linkers = obj.Lang.Equals("Sharp") ? generateLinkers(obj) : "";

            string gentext = String.Format("" +
                "protected:\n" +
                "{3}" +
               "   virtual void resolve(ScriptBuilder script) override\n" +
               "{{\n" +
               "{4}\n\n"+
               "{0}\n\n" +
               "{1}\n\n" +
               "{2}\n\n" +
               "}}", variables.ToString(), methods.ToString(), properties.ToString(), pythonCalls, linkers);

            return gentext;
        }

        /*
         * 
         

    static MonoString* get_output_text(MonoString* prefix, int* number)
	{
		std::string cpp_prefix = TypeConversion<std::string>(prefix).Value;
		IObject* pObject = runtime.getObject("python_file");
		python_file* pCasted = static_cast<python_file*>(pObject);
		std::string result = pCasted->get_output_text->invoke(cpp_prefix, *number);
		return TypeConversion<std::string>(result).pMonoValue;
	}

	static void print_custom_text(MonoString* text)
	{
		std::string cpp_text = TypeConversion<std::string>(text).Value;
		IObject* pObject = runtime.getObject("python_file");
		python_file* pCasted = static_cast<python_file*>(pObject);
		pCasted->print_custom_text->invoke(cpp_text);
         */

        public String generatePythonMethodCalls(FormedObject obj)
        {
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i<obj.PyUsages.Count; i++)
            {
                String returnType = obj.PyUsages[i].Methods.ReturnType.ToLower().Equals("string") ? "MonoString* " : " " +obj.PyUsages[i].Methods.ReturnType;
                builder.Append("static " + returnType + obj.PyUsages[i].Methods.Name+"(");
                List<ParamInfo> parameters = obj.PyUsages[i].Methods.Params;
                for (int j = 0; j < parameters.Count; j++)
                {
                    String type = parameters[j].Type.ToLower().Equals("string") ? "MonoString* " : parameters[j].Type + "*";
                    builder.Append(type + " " + parameters[j].Name);
                    if (j != parameters.Count - 1) builder.Append(",");
                }
                builder.Append(")\n{\n");

                for(int j = 0; j<parameters.Count; j++)
                {
                    if (parameters[j].Type.ToLower().Equals("string"))
                    {
                        builder.Append("std::string cpp_" + parameters[j].Name + " = " + "TypeConversion<std::string>(" + parameters[j].Name + ").Value;\n");
                    }
                }
                builder.Append("IObject* pObject = runtime.getObject(\""+obj.PyUsages[i].fromFile+"\");\n");
                builder.Append(obj.PyUsages[i].fromFile + "* pCasted = static_cast<" + obj.PyUsages[i].fromFile + "*>(pObject);\n");

                if (!obj.PyUsages[i].Methods.ReturnType.Equals("void"))
                {
                    builder.Append(fieldMapper(obj.PyUsages[i].Methods.ReturnType) + " result = ");
                }
                builder.Append("pcasted->" + obj.PyUsages[i].Methods.Name+"->invoke(");
                for(int j = 0; j<parameters.Count; j++)
                {
                    String parameter = parameters[j].Type.ToLower().Equals("string") ? "cpp_" + parameters[j].Name : "*" + parameters[j].Name;
                    builder.Append(parameter);
                    if (j != parameters.Count - 1) builder.Append(",");
                }
                builder.Append(");\n");

                if (!obj.PyUsages[i].Methods.ReturnType.Equals("void"))
                {
                    String functionReturn = obj.PyUsages[i].Methods.ReturnType.ToLower().Equals("string") ? "TypeConversion<std::string>(result).pMonoValue;" : "result";
                    builder.Append("return "+ functionReturn);
                }
                builder.Append("}\n\n");
            }
            return builder.ToString();
        }

        /*
         * 
         * 
         * 
        invokeDefaultCtor();

		auto par = script.resolveField<PythonInvokeTest*>("Instance");
		script.linkFunction("__internal_get_output_text", PythonInvokeTest::get_output_text);
		script.linkFunction("__internal_print_custom_text", PythonInvokeTest::print_custom_text);
		par->setValue(this);
         */
        public String generateLinkers(FormedObject obj)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("invokeDefaultCtor();\n\n");

            builder.Append("auto par = script.resolveField<" + obj.Class.Name + "*>(\"Instance\");\n");
            for(int i = 0; i<obj.PyUsages.Count; i++)
            {
                builder.Append("script.linkFunction(\"__internal_" + obj.PyUsages[i].Methods.Name+"\", " + obj.Class.Name+"::"+obj.PyUsages[i].Methods.Name+");\n");
            }
            builder.Append("par->setValue(this);");
            return builder.ToString();
        }

       

        String generateInstanceId(FormedObject obj)
        {
            return String.Format("\n\nvirtual std::string getInstanceID() const override{{\n\treturn \"{0}\";\n}}", obj.Class.Name);
        }

        private StringBuilder PropertiesResult(FormedObject obj, bool memoryRef = false)
        {
            StringBuilder properties = new StringBuilder();
            if(obj.Properties != null)
            {
                foreach (PropertyInfo p in obj.Properties)
                {
                    properties.Append(
                        !memoryRef ?
                        String.Format("{0} = script.resolverProperty<{1}>(\"{0}\");\n", p.Name, fieldMapper(p.Type)) :
                        String.Format("Memory::reference<Property<{1}>> {0};", p.Name, fieldMapper(p.Type)));
                }

            }

            return properties;
        }

        private StringBuilder MethodsResult(FormedObject obj, bool memoryRef = false)
        {
            //Methods
            StringBuilder methods = new StringBuilder();
            foreach (MethodInfo m in obj.Methods)
            {
                String parameters = "";
                String simpleParams = "";
                if (m.Params.Count > 0)
                {
                    StringBuilder paramsBuilder = new StringBuilder();
                    StringBuilder simpleParamsBuilder = new StringBuilder();
                    foreach (ParamInfo p in m.Params)
                    {
                        simpleParamsBuilder.Append(p.Type + ",");
                        paramsBuilder.Append("," + fieldMapper(p.Type));
                    }
                    simpleParams = simpleParamsBuilder.ToString();
                    simpleParams = simpleParams.Substring(0, simpleParams.Length - 1);
                    parameters = paramsBuilder.ToString();
                }


                String field = !memoryRef ? 
                    String.Format("{0} = script.resolveMethod<{1}{2}>(\"{0}({3})\");\n", m.Name, fieldMapper(m.ReturnType), parameters, simpleParams) :
                    String.Format("Memory::reference<Method<{1}{2}>> {0};\n", m.Name, fieldMapper(m.ReturnType), parameters)
                    ;
                methods.Append(field);
            }

            return methods;
        }


        private StringBuilder VariablesResult(FormedObject obj, bool memoryRef = false)
        {
            //Variables
            StringBuilder variables = new StringBuilder();
            foreach (PropertyInfo p in obj.Variables)
            {
                String field = memoryRef ? 
                    String.Format("Memory::reference<Field<{0}>> {1};\n", fieldMapper(p.Type), p.Name):
                    String.Format("{0} = script.resolvefield<{1}>(\"{0}\");\n", p.Name, fieldMapper(p.Type));

                variables.Append(field);
            }

            return variables;
        }


        String fieldMapper(String typeName)
        {
            switch (typeName)
            {
                case "int":
                    return "int";
                case "string":
                case "String":
                    return "std::string";
                case "void":
                    return "void";
                default:
                    return "";
            }
        }
    }
}
