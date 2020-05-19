using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Xlang.SharpDetection
{
    class MethodInfo
    {
        public bool IsStatic { get; set; }
        public String Name { get; set; }
        public String ReturnType { get; set; }
        public String AccessModifier { get; set; }
        public List<ParamInfo> Params;

        public MethodInfo(bool isStatic, string name, string returnType, string accessModifier, List<ParamInfo> @params)
        {
            IsStatic = isStatic;
            Name = name;
            ReturnType = returnType;
            AccessModifier = accessModifier;
            Params = @params;
        }
    }
}
