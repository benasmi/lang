using System;

class TestScript
{
    int FieldTest = 50;
    string FieldTestString = "string";
    static int FieldTestStatic = 100;
    static string FieldTestStringStatic = "static_string";

    int pA { get; set; } = 200;
    static int pB { get; set; } = 500;

    string spA { get; set; } = "instance property";
    static string spB { get; set; } = "static property";

    static void voidTestMethodStatic()
    {
        Console.WriteLine("Static void test method!");
    }

    void voidTestMethod()
    {
        Console.WriteLine("Void test method!");
    }

    void voidTestMethodParam(string Text)
    {
        string testing = "testing";
        Console.WriteLine(Text);
    }

    void voidTestMethodEx()
    {
        throw new ArgumentException("Test exception!");
    }

    int intTestMethod()
    {
        return 10;
    }

    string stringTestMethod()
    {
        return "C# string";
    }

    int intTestMethodParam(int Ret)
    {
        return Ret;
    }
}