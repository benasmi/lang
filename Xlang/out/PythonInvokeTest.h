class PythonInvokeTest : public ScriptObject
{
public:
   PythonInvokeTest() : ScriptObject("sharp_glue","SharpGlue","PythonInvokeTest"){}

virtual std::string getInstanceID() const override{
	return "PythonInvokeTest";
}
protected:

static MonoString* get_output_text(MonoString*  prefix,int* number)
{
std::string cpp_prefix = TypeConversion<std::string>(prefix).Value;
IObject* pObject = runtime.getObject("python_file");
python_file* pCasted = static_cast<python_file*>(pObject);
std::string result = pcasted->get_output_text->invoke(cpp_prefix,*number);
return TypeConversion<std::string>(result).pMonoValue;
}

static  voidprint_custom_text(MonoString*  text)
{
std::string cpp_text = TypeConversion<std::string>(text).Value;
IObject* pObject = runtime.getObject("python_file");
python_file* pCasted = static_cast<python_file*>(pObject);
pcasted->print_custom_text->invoke(cpp_text);
}

virtual void resolve(ScriptBuilder script) override
{
invokeDefaultCtor();

auto par = script.resolveField<PythonInvokeTest*>("Instance");
script.linkFunction("__internal_get_output_text", PythonInvokeTest::get_output_text);
script.linkFunction("__internal_print_custom_text", PythonInvokeTest::print_custom_text);
par->setValue(this);

tekstas = script.resolvefield<std::string>("tekstas");

Main = script.resolveMethod<void>("Main(string[])");

}
public:

Memory::reference<Field<std::string>> tekstas;
Memory::reference<Method<void>> Main;

};