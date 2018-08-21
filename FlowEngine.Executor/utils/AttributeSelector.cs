using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Executor.utils
{
    /// <summary>
    /// 
    /// @author: Vincent Nacar
    /// </summary>
    public abstract class AttributeSelector
    {
        public object valueOf(string selectorValue)
        {
            object value = null;
            if (selectorValue.StartsWith("@") && selectorValue.EndsWith("@"))
            {
                string variableKey = selectorValue.Replace("@", "");
                value = getVariableRegistry()[variableKey];
            }
            else if (selectorValue.StartsWith("[") && selectorValue.EndsWith("]"))
            {
                string activityFieldSelector = selectorValue.Replace("[", "").Replace("]", "");

                if (!activityFieldSelector.Contains("."))
                    throw new Exception("Invalid value-of selector.");

                string[] activityField = activityFieldSelector.Split('.');

                IResult result = getActivityInstances()[activityField[0]];
                value = result.getData()[activityField[1]];
            }
            else
            {
                value = selectorValue;
            }
            return value;
        }

        protected abstract IDictionary<object, object> getVariableRegistry();

        protected abstract IDictionary<object, IResult> getActivityInstances();
    }
}
