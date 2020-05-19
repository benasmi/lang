using System;
using System.Collections.Generic;
using System.Text;

namespace Xlang
{
    class ClassInfo
    {
        public String Name { get; set; }
        public String AccessModifier { get; set; }
        public bool IsStatic { get; set; }

        public ClassInfo(string name, string accessModifier, bool isStatic)
        {
            Name = name;
            AccessModifier = accessModifier;
            IsStatic = isStatic;
        }
    }
}
