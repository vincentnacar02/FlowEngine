using FlowEngine.SDK.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core.providers
{
    /// <summary>
    /// WorkflowStateProvider
    /// @author: Vincent Nacar
    /// </summary>
    public class WorkflowStateProvider : IWorkflowState
    {
        private IDictionary<object, IActivity> ActivitiesMap;
        private IDictionary<object, IResult> ActivitiesResultsMap;
        private IDictionary<object, object> Variables;

        public WorkflowStateProvider()
        {
            ActivitiesMap = new Dictionary<object, IActivity>();
            ActivitiesResultsMap = new Dictionary<object, IResult>();
            Variables = new Dictionary<object, object>();
        }

        public IDictionary<object, IActivity> getActivities()
        {
            return this.ActivitiesMap;
        }

        public IDictionary<object, IResult> getActivityResults()
        {
            return this.ActivitiesResultsMap;
        }

        public IDictionary<object, object> getVariables()
        {
            return this.Variables;
        }
    }
}
