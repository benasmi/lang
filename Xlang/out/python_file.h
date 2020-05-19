class python_file : public PythonObject
{
public:
   python_file() : PythonObject("python_file"){}


virtual std::string getInstanceID() const override{
	return "python_file";
}protected:
   virtual void resolve(ScriptBuilder script) override
{




get_output_text = script.resolveMethod<std::string,std::string,int>("get_output_text(string,int)");
print_custom_text = script.resolveMethod<void,std::string>("print_custom_text(string)");




}public:



Memory::reference<Method<std::string,std::string,int>> get_output_text;
Memory::reference<Method<void,std::string>> print_custom_text;




};