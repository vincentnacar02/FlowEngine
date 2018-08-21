using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    /// <summary>
    /// Abstract Activity class
    /// </summary>
    public abstract class Activity : IActivity
    {
        private object _id { get; set; }
        private IProperties _properties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Id">Id of activity</param>
        /// <param name="props">Properties of Activity</param>
        public Activity(object Id, IProperties props)
        {
            this._id = Id;
            this._properties = props;
        }

        /// <summary>
        /// Get Id of activity
        /// </summary>
        /// <returns>id</returns>
        public object getId()
        {
            return this._id;
        }

        /// <summary>
        /// Run the activity
        /// </summary>
        /// <returns>IResult instance</returns>
        public abstract IResult run();

        /// <summary>
        /// Get Properties of Activity
        /// </summary>
        /// <returns>IProperties</returns>
        public IProperties getProperties()
        {
            return this._properties;
        }

        /// <summary>
        /// Set Property value
        /// </summary>
        /// <param name="key">name of the Property</param>
        /// <param name="value">new value of the Property object</param>
        public void setPropertyValue(string key, object value)
        {
            IProperty prop = this._properties.getProperty(key);
            prop.setValue(value);
            this._properties.getItems()[key] = prop;
        }

        /// <summary>
        /// A hook that being called when initializing workflow
        /// </summary>
        public abstract void onInit();

        /// <summary>
        /// Provide an access to workflow global settings
        /// </summary>
        /// <param name="provider">provider</param>
        public virtual void onInitSettings(SettingsProvider provider) { }
    }
}
