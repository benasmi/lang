using System;
using System.Collections.Generic;
using System.Text;

namespace Xlang.SharpDetection
{
    class ParamInfo
    {
        public String Type { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }

        public ParamInfo(string type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}
