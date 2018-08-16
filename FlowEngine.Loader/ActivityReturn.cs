using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Loader
{
    public class ActivityReturn
    {
        public String ActivityId { get; set; }
        public String ReturnField { get; set; }
        public object ReturnValue { get; set; }
        public String ReturnType { get; set; }

        public ActivityReturn(string activityId, string returnField, object returnValue, string returnType)
        {
            ActivityId = activityId;
            ReturnField = returnField;
            ReturnValue = returnValue;
            ReturnType = returnType;
        }
    }
}
