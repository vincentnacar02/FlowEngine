using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK.interfaces
{
    public interface IActivity
    {
        object getId();

        IResult run();

        void setPropertyValue(string key, object value);

        // lifecycle hooks
        void onInit();
    }
}
