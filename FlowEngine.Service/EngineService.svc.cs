using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FlowEngine.Service
{
    public class EngineService : IEngineService
    {
        public void StartWorkflow(Workflow workflow)
        {
            try
            {
                //TEST
                string flowEngine = System.Configuration.ConfigurationManager.AppSettings["WorkflowEngineExe"];
                string workflowDir = System.Configuration.ConfigurationManager.AppSettings["WorkflowDirectory"];
                string workflowPath = System.IO.Path.Combine(workflowDir, "testrepeatworkflow.xml");

                System.Diagnostics.ProcessStartInfo workflowInfo = new System.Diagnostics.ProcessStartInfo(flowEngine);
                workflowInfo.Arguments = "\"" + workflowPath + "\"";
                workflowInfo.UseShellExecute = false;

                System.Diagnostics.Process.Start(workflowInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Workflow> GetWorkflows()
        {
            Workflow test = new Workflow();
            List<Workflow> tests = new List<Workflow>();
            tests.Add(test);
            return tests;
        }
    }
}
