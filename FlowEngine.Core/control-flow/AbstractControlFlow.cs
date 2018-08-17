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
                bool valid = this.validateAttribute(attribute);
                if (valid)
                {
                    ElementAttribute _elementAttr = new ElementAttribute(attribute.Name, attribute.Value);
                    this.Attributes.Add(attribute.Name, _elementAttr);
                }
            }
        }

        public void ValidateRequiredAttributes()
        {
            string[] _missingAttributes = new string[] { };
            int i = 0;
            foreach (var attributeName in this.getRequiredAttributes())
            {
                if (!this.Attributes.ContainsKey(attributeName))
                {
                    _missingAttributes[i] = attributeName;
                    i++;
                }
            }
            if (i > 0)
            {
                throw new Exception(string.Format("Missing attributes {0} in element {1}", string.Join(",", _missingAttributes), this.getControlFlowType().ToString()));
            }
        }

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

        protected abstract bool validateAttribute(XmlAttribute attribute);

        public abstract string[] getRequiredAttributes();
    }
}
