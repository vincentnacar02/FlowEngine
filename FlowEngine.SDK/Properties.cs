using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    /// <summary>
    /// Contains dictionary of Property
    /// </summary>
    public class Properties : IProperties
    {
        private IDictionary<string, IProperty> _properties;
        public Properties()
        {
            this._properties = new Dictionary<string, IProperty>();
        }

        /// <summary>
        /// Add Property object
        /// </summary>
        /// <param name="prop">property object</param>
        public void addProperty(IProperty prop)
        {
            this._properties.Add(prop.getName(), prop);
        }

        /// <summary>
        /// Get Property object
        /// </summary>
        /// <param name="key">the name of object</param>
        /// <returns></returns>
        public IProperty getProperty(string key)
        {
            return this._properties[key];
        }

        /// <summary>
        /// Get all properties
        /// </summary>
        /// <returns>IDictionary<string, IProperty></returns>
        public IDictionary<string, IProperty> getItems()
        {
            return this._properties;
        }
    }
}
