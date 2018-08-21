using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    /// <summary>
    /// Property definition used by Properties injected in Activity
    /// </summary>
    public class Property : IProperty
    {
        private string _name { get; set; }
        private object _value { get; set; }

        /// <summary>
        /// Property Constructor
        /// </summary>
        /// <param name="name">Name of Property</param>
        /// <param name="value">Value of Property</param>
        public Property(string name, string value)
        {
            this._name = name;
            this._value = value;
        }

        /// <summary>
        /// Returns name of Property
        /// </summary>
        /// <returns>returns name</returns>
        public string getName()
        {
            return this._name;
        }

        /// <summary>
        /// Returns value of Property
        /// </summary>
        /// <returns>returns value</returns>
        public object getValue()
        {
            return this._value;
        }

        /// <summary>
        /// Set value of a Property
        /// </summary>
        /// <param name="value">property value</param>
        public void setValue(object value)
        {
            this._value = value;
        }
    }
}
