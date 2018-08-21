using FlowEngine.Core.constants;
using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core.input
{
    public class AssignElement : AbstractElement
    {
        private string[] VALID_ATTRIBUTES = new string[] { "type", "to", "from" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "type", "to", "from" };

        public AssignElement(XmlNode node)
            : base(node.Attributes)
        {

        }

        public override string getElementName()
        {
            return ElementNameConstants.ASSIGN;
        }

        public override ElementType getType()
        {
            return ElementType.Input;
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
