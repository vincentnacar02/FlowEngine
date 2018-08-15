using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core
{
    public class Properties : IProperties
    {
        private IDictionary<string, IProperty> _properties;
        public Properties()
        {
            this._properties = new Dictionary<string, IProperty>();
        }

        public void addProperty(IProperty prop)
        {
            this._properties.Add(prop.getName(), prop);
        }

        public IProperty getProperty(string key)
        {
            return this._properties[key];
        }
    }
}
