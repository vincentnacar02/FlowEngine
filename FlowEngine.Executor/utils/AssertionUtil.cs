using FlowEngine.Executor.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Executor.utils
{
    public class AssertionUtil
    {
        public static Boolean AssertCondition(ConditionType condition, object expectedValue, object actualValue)
        {
            Boolean result = false;
            switch (condition)
            {
                case ConditionType.EqualsTo:
                    result = AssertionUtil.equals(expectedValue, actualValue);
                    break;
                case ConditionType.NotEqualsTo:
                    result = AssertionUtil.notEquals(expectedValue, actualValue);
                    break;
                default:
                    break;
            }
            return result;
        }

        private static Boolean equals(object expectedValue, object actualValue)
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

        private static Boolean notEquals(object expectedValue, object actualValue)
        {
            Boolean eqResult = false;
            if (actualValue.GetType() == typeof(Boolean))
            {
                eqResult = Convert.ToBoolean(expectedValue) != (Boolean)actualValue;
            }
            else if (actualValue.GetType() == typeof(String))
            {
                eqResult = !expectedValue.Equals(actualValue);
            }
            return eqResult;
        }
    }
}
