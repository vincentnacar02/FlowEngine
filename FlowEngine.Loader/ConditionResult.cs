using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Executor
{
    public class ConditionResult
    {
        public Boolean Result { get; set; }
        public XmlNodeList DoNodes { get; set; }
        public XmlNodeList ElseNodes { get; set; }

        public ConditionResult(Boolean result, XmlNodeList doNodes, XmlNodeList elseNodes)
        {
            Result = result;
            DoNodes = doNodes;
            ElseNodes = elseNodes;
        }
    }
}
