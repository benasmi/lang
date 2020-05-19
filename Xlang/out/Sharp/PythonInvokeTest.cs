%python_file%

using System;
using System.Runtime.CompilerServices;


namespace Project
{
	class PythonInvokeTest
	{
	    IntPtr Instance;
[MethodImpl(MethodImplOptions.InternalCall)]
extern private static string __internal_get_output_text(  string prefix,out int number);

[MethodImpl(MethodImplOptions.InternalCall)]
extern private static void __internal_print_custom_text(  string text);

static void Main(string[] Args)
	    {
	string __prefix = "Skaicius";
int __number = 50;
        string tekstas = __internal_get_output_text(__prefix,out __number);
	string __text = tekstas;
__internal_print_custom_text(__text);
	    }
	}
}