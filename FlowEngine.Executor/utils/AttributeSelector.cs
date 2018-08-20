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
                string activityId = selectorValue.Replace("[", "").Replace("]", "");
                ActivityReturn activityReturn = getActivityInstances()[activityId];
                value = activityReturn.ReturnValue;
            }
            else
            {
                value = selectorValue;
            }
            return value;
        }

        protected abstract IDictionary<object, object> getVariableRegistry();

        protected abstract IDictionary<object, ActivityReturn> getActivityInstances();
    }
}
