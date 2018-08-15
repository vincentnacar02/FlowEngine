using FlowEngine.Core;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sum
{
    public class Sum : Activity
    {
        public Sum(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
                var input1 = this.getProperties().getProperty("InputNumber1");
                var input2 = this.getProperties().getProperty("InputNumber2");
                if (input1 != null && input2 != null)
                {
                    Console.WriteLine("Sum is {0}", computeSum(input1, input2));
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(null, ResultStatus.SUCCESS);
        }

        public Int32 computeSum(IProperty num1, IProperty num2)
        {
            return Convert.ToInt32(num1.getValue()) + Convert.ToInt32(num2.getValue());
        }
    }
}
