using System;
using System.Collections.Generic;
using System.Text;
using Xlang.StructureObjects;

namespace Xlang.SharpDetection
{
    class FormedObject
    {
        public ClassInfo Class;
        public String Lang;
        public List<PropertyInfo> Properties;
        public List<PropertyInfo> Variables;
        public List<MethodInfo> Methods;
        public List<PyUsage> PyUsages;

        public FormedObject(ClassInfo @class, string lang, List<PropertyInfo> properties, List<PropertyInfo> variables, List<MethodInfo> methods, List<PyUsage> pyUsages)
        {
            Class = @class;
            Lang = lang;
            Properties = properties;
            Variables = variables;
            Methods = methods;
            PyUsages = pyUsages;
        }
    }
}
