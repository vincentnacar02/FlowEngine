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
        public XmlNodeList DoNodes { get; set; }
        public XmlNodeList ElseNodes { get; set; }

        private string[] VALID_ATTRIBUTES = new string[] { "activityId", "condition", "value" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "activityId", "condition", "value" };

        public IfElement(XmlNode ifNode, XmlNodeList doNodes, XmlNodeList elseNodes)
            : base(ifNode.Attributes)
        {
            this.DoNodes = doNodes;
            this.ElseNodes = elseNodes;
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
