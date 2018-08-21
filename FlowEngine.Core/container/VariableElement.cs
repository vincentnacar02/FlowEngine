using FlowEngine.Core.constants;
using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core.container
{
    /// <summary>
    /// VariableElement
    /// @author: Vincent Nacar
    /// </summary>
    public class VariableElement : AbstractElement
    {
        private string[] VALID_ATTRIBUTES = new string[] { "name", "type", "value" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "name", "value" };

        public VariableElement(XmlNode node)
            : base(node.Attributes)
        {

        }

        public override string getElementName()
        {
            return ElementNameConstants.VARIABLE;
        }

        public override ElementType getType()
        {
            return ElementType.Container;
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
