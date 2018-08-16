using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core
{
    public abstract class Activity : IActivity
    {
        private object _id { get; set; }
        private IProperties _properties;

        public Activity(object Id, IProperties props)
        {
            this._id = Id;
            this._properties = props;
        }

        public object getId()
        {
            return this._id;
        }

        public abstract IResult run();

        public IProperties getProperties()
        {
            return this._properties;
        }

        public void setPropertyValue(string key, object value)
        {
            IProperty prop = this._properties.getProperty(key);
            prop.setValue(value);
            this._properties.getItems()[key] = prop;
        }
    }
}
