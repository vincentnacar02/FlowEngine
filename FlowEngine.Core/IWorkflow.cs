using FlowEngine.Core.providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core
{
    /// <summary>
    /// IWorkflow
    /// @author: VincentNacar
    /// </summary>
    public interface IWorkflow
    {
        void InitializeElements();

        void SetState(IWorkflowState stateProvider);

        IWorkflowState GetState();
    }
}
