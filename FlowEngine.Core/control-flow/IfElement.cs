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
    /// IfElement
    /// @author: Vincent Nacar
    /// </summary>
    public class IfElement : AbstractControlFlow
    {
        public IList<IElement> DoNodes { get; set; }
        public IList<IElement> ElseNodes { get; set; }

        private string[] VALID_ATTRIBUTES = new string[] { "value-of", "condition", "value" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "value-of", "condition", "value" };

        public IfElement(XmlNode ifNode, IList<IElement> doNodes, IList<IElement> elseNodes)
            : base(ifNode.Attributes)
        {
            this.DoNodes = doNodes;
            this.ElseNodes = elseNodes;
        }

        public override string getElementName()
        {
            return "If";
        }

        public override ControlFlowType getControlFlowType()
        {
            return ControlFlowType.If;
        }

        public override bool hasDoNodes()
        {
            return this.DoNodes != null && this.DoNodes.Count > 0;
        }

        public override bool hasElseNodes()
        {
            return this.ElseNodes != null & this.ElseNodes.Count > 0;
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
