using FlowEngine.Service.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FlowEngine.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class EngineService : IEngineService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EngineService));

        public void StartWorkflow(Workflow workflow)
        {
            try
            {
                string flowEngine = System.Configuration.ConfigurationManager.AppSettings["WorkflowEngineExe"];
                string workflowDir = System.Configuration.ConfigurationManager.AppSettings["WorkflowDirectory"];
                string workflowPath = System.IO.Path.Combine(workflowDir, workflow.FileName.ToString());

                System.Diagnostics.ProcessStartInfo workflowInfo = new System.Diagnostics.ProcessStartInfo(flowEngine);
                workflowInfo.Arguments = "\"" + workflowPath + "\"";
                workflowInfo.UseShellExecute = false;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = workflowInfo;
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => 
                {
                    System.Diagnostics.Process owner = sender as System.Diagnostics.Process;
                    log.InfoFormat("Process ExitCode : {0}", owner.ExitCode);
                    log.InfoFormat("Process End. {0}", workflow.FileName);
                };

                System.Threading.ThreadStart _threadStart = new System.Threading.ThreadStart(() =>
                {
                    log.InfoFormat("Process start. {0}" , workflow.FileName);
                    bool result = process.Start();
                });
                System.Threading.Thread _thread = new System.Threading.Thread(_threadStart);
                _thread.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<Workflow> GetWorkflows()
        {
            return Global.WorkflowsIntance;
        }
    }
}
