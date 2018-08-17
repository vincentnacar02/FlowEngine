using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    public class Property : IProperty
    {
        private string _name { get; set; }
        private object _value { get; set; }

        public Property(string name, string value)
        {
            this._name = name;
            this._value = value;
        }

        public string getName()
        {
            return this._name;
        }

        public object getValue()
        {
            return this._value;
        }

        public void setValue(object value)
        {
            this._value = value;
        }
    }
}
