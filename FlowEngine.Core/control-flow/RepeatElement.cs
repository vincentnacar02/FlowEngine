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
    /// RepeatElement
    /// @author: Vincent Nacar
    /// </summary>
    public class RepeatElement : AbstractControlFlow
    {
        public XmlNodeList DoNodes { get; set; }

        private string[] VALID_ATTRIBUTES = new string[] {"times"};
        private string[] REQUIRED_ATTRIBUTES = new string[] { "times" };

        public RepeatElement(XmlNode node, XmlNodeList doNodes)
            : base(node.Attributes)
        {
            this.DoNodes = doNodes;
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

        protected override bool validateAttribute(XmlAttribute attribute)
        {
            return this.VALID_ATTRIBUTES.Contains(attribute.Name);
        }

        public override string[] getRequiredAttributes()
        {
            return this.REQUIRED_ATTRIBUTES;
        }
    }
}
