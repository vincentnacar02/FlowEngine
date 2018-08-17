using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Executor.utils
{
    public class AssertionUtil
    {
        public static Boolean equals(object expectedValue, object actualValue)
        {
            Boolean eqResult = false;
            if (actualValue.GetType() == typeof(Boolean))
            {
                eqResult = Convert.ToBoolean(expectedValue) == (Boolean) actualValue;
            }
            else if (actualValue.GetType() == typeof(String))
            {
                eqResult = expectedValue.Equals(actualValue);
            }
            return eqResult;
        }
    }
}
