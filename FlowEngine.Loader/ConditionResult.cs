using FlowEngine.Core.elements.interfaces;
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
        public IList<IElement> DoNodes { get; set; }
        public IList<IElement> ElseNodes { get; set; }

        public ConditionResult(Boolean result, IList<IElement> doNodes, IList<IElement> elseNodes)
        {
            Result = result;
            DoNodes = doNodes;
            ElseNodes = elseNodes;
        }
    }
}
