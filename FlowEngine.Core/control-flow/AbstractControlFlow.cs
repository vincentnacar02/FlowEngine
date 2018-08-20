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
    /// AbstractControlFlow
    /// @author: Vincent Nacar
    /// </summary>
    public abstract class AbstractControlFlow : IElement
    {
        private IDictionary<string, IAttribute> Attributes;
        private XmlAttributeCollection XmlAttributes;

        public AbstractControlFlow(XmlAttributeCollection attributes)
        {
            Attributes = new Dictionary<string, IAttribute>();
            XmlAttributes = attributes;
        }

        public void InitializeElement()
        {
            foreach (XmlAttribute attribute in this.XmlAttributes)
            {
                // validate required 
                bool valid = this.getValidAttributes().Contains(attribute.Name);
                if (valid)
                {
                    ElementAttribute _elementAttr = new ElementAttribute(attribute.Name, attribute.Value);
                    this.Attributes.Add(attribute.Name, _elementAttr);
                }
            }
        }

        public void ValidateRequiredAttributes()
        {
            string _missingAttribute = null;
            foreach (var attributeName in this.getRequiredAttributes())
            {
                if (!this.Attributes.ContainsKey(attributeName))
                {
                    _missingAttribute = attributeName;
                }
            }
            if (_missingAttribute != null)
            {
                throw new Exception(string.Format("Missing attribute {0} in element {1}", _missingAttribute, this.getElementName().ToString()));
            }
        }

        public abstract string getElementName();

        public ElementType getType()
        {
            return ElementType.ControlFlow;
        }

        public ICollection<IAttribute> getAttributes()
        {
            return this.Attributes.Values;
        }

        public IAttribute getAttribute(string name)
        {
            IAttribute _attr = null;
            if (this.Attributes.ContainsKey(name))
            {
                _attr = this.Attributes[name];
            }
            return _attr;
        }

        public abstract ControlFlowType getControlFlowType();

        public abstract bool hasDoNodes();

        public abstract bool hasElseNodes();

        public abstract string[] getValidAttributes();

        public abstract string[] getRequiredAttributes();
    }
}
