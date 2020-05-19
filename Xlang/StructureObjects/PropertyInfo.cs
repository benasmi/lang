using System;
using System.Collections.Generic;
using System.Text;

namespace Xlang
{
    class PropertyInfo
    {
        public String Name { get; set; }
        public String Type { get; set; }
        public String AccessModifier { get; set; }
        public bool IsStatic { get; set; }

        public PropertyInfo(string name, string type, string accessModifier, bool isStatic)
        {
            Name = name;
            Type = type;
            AccessModifier = accessModifier;
            IsStatic = isStatic;
        }
    }
}
