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
        public XmlNodeList DoNodes { get; set; }

        private string[] VALID_ATTRIBUTES = new string[] { "activityId","as" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "activityId", "as" };

        public ForEachElement(XmlNode node, XmlNodeList doNodes) : base(node.Attributes)
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
