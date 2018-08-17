using FlowEngine.Core.elements.interfaces;
using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core.control_flow
{
    /// <summary>
    /// ForEachElement
    /// @author: Vincent Nacar
    /// </summary>
    public class ForEachElement : AbstractControlFlow
    {
        public IList<IElement> DoNodes { get; set; }

        private string[] VALID_ATTRIBUTES = new string[] { "activityId","as" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "activityId", "as" };

        public ForEachElement(XmlNode node, IList<IElement> doNodes)
            : base(node.Attributes)
        {
            this.DoNodes = doNodes;
        }

        public override string getElementName()
        {
            return "ForEach";
        }

        public override ControlFlowType getControlFlowType()
        {
            return ControlFlowType.ForEach;
        }

        public override bool hasDoNodes()
        {
            return this.DoNodes != null && this.DoNodes.Count > 0;
        }

        public override bool hasElseNodes()
        {
            return false;
        }

        public override string[] getValidAttributes()
        {
            return this.VALID_ATTRIBUTES;
        }

        public override string[] getRequiredAttributes()
        {
            return this.REQUIRED_ATTRIBUTES;
        }
    }
}
