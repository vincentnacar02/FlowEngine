using FlowEngine.Core.constants;
using FlowEngine.Core.elements.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core.activity
{
    /// <summary>
    /// ActivityElement
    /// @author: Vincennt Nacar
    /// </summary>
    public class ActivityElement : AbstractElement
    {
        private string[] VALID_ATTRIBUTES = new string[] { "id" };
        private string[] REQUIRED_ATTRIBUTES = new string[] { "id" };

        public ActivityElement(XmlNode node)
            : base(node.Attributes)
        {

        }

        public override string getElementName()
        {
            return ElementNameConstants.ACTIVITY;
        }

        public override ElementType getType()
        {
            return ElementType.Activity;
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
