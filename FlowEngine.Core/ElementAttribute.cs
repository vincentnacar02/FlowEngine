using FlowEngine.Core.elements.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core
{
    public class ElementAttribute : IAttribute
    {
        private string Name;
        private object Value;

        public ElementAttribute(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string getName()
        {
            return this.Name;
        }

        public object getValue()
        {
            return this.Value;
        }
    }
}
