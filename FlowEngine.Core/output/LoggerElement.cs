using FlowEngine.Core.constants;
using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core.output
{
    public class LoggerElement : AbstractElement
    {
        private string[] VALID_ATTRIBUTES = new string[] { "type", "value" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "type", "value" };

        public LoggerElement(XmlNode node)
            : base(node.Attributes)
        {

        }

        public override string getElementName()
        {
            return ElementNameConstants.LOGGER;
        }

        public override ElementType getType()
        {
            return ElementType.Output;
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
