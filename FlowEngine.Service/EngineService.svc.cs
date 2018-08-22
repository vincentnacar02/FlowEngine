﻿using FlowEngine.Service.models;
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

                System.Threading.ThreadStart _threadStart = new System.Threading.ThreadStart(() =>
                {
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
