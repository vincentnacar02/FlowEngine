using FlowEngine.Core;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs
{
    public class HelloWorld : Activity
    {
        public HelloWorld(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
                var input = this.getProperties().getProperty("InputName");
                if (input != null)
                {
                    Console.WriteLine("Hello World!, {0}", input.getValue());
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(null, ResultStatus.SUCCESS);
        }
    }
}
