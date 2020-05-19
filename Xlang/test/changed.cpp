class TestScript : public ScriptObject
{
public:
    TestScript() : ScriptObject("sharp_glue", "SharpGlue", "TestScript") {}
protected:
    virtual void resolve(ScriptBuilder script) override
    {
        FieldTest = script.resolvefield<int>("FieldTest");
        FieldTestString = script.resolvefield<std::string>("FieldTestString");
        FieldTestStatic = script.resolvefield<int>("FieldTestStatic");
        FieldTestStringStatic = script.resolvefield<std::string>("FieldTestStringStatic");
        testing = script.resolvefield<std::string>("testing");


        voidTestMethodStatic = script.resolveMethod<void>("voidTestMethodStatic()");
        voidTestMethod = script.resolveMethod<void>("voidTestMethod()");
        voidTestMethodParam = script.resolveMethod<void, std::string>("voidTestMethodParam(string)");
        voidTestMethodEx = script.resolveMethod<void>("voidTestMethodEx()");
        intTestMethod = script.resolveMethod<int>("intTestMethod()");
        stringTestMethod = script.resolveMethod<std::string>("stringTestMethod()");
        intTestMethodParam = script.resolveMethod<int, int>("intTestMethodParam(int)");


        pA = script.resolverProperty<int>("pA");
        pB = script.resolverProperty<int>("pB");
        spA = script.resolverProperty<std::string>("spA");
        spB = script.resolverProperty<std::string>("spB");


    }public:

        Memory::reference<Field<int>> FieldTest;
        Memory::reference<Field<std::string>> FieldTestString;
        Memory::reference<Field<int>> FieldTestStatic;
        Memory::reference<Field<std::string>> FieldTestStringStatic;
        Memory::reference<Field<std::string>> testing;


        Memory::reference<Method<void>> voidTestMethodStatic;
        Memory::reference<Method<void>> voidTestMethod;
        Memory::reference<Method<void, std::string>> voidTestMethodParam;
        Memory::reference<Method<void>> voidTestMethodEx;
        Memory::reference<Method<int>> intTestMethod;
        Memory::reference<Method<std::string>> stringTestMethod;
        Memory::reference<Method<int, int>> intTestMethodParam;


        Memory::reference<Property<int>> pA; Memory::reference<Property<int>> pB; Memory::reference<Property<std::string>> spA; Memory::reference<Property<std::string>> spB;

};