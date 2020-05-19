using System;
using System.Collections.Generic;
using System.Text;
using Xlang.SharpDetection;

namespace Xlang.StructureObjects
{
    class PyUsage
    {
        public MethodInfo Methods;
        public String fromFile { get; set; }

        public PyUsage(MethodInfo methods, String fromFile)
        {
            Methods = methods;
            this.fromFile = fromFile;
        }
    }
}
