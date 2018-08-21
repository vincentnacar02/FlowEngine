using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core.providers
{
    public interface IWorkflowState
    {
        IDictionary<object, IActivity> getActivities();

        IDictionary<object, IResult> getActivityResults();

        IDictionary<object, object> getVariables();
    }
}
