using FlowEngine.Service.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml;

namespace FlowEngine.Service
{
    public class Global : System.Web.HttpApplication
    {
        public static IList<Workflow> WorkflowsIntance;

        protected void Application_Start(object sender, EventArgs e)
        {
            if (WorkflowsIntance == null) {
                WorkflowsIntance = new List<Workflow>();
            }

            XmlDocument _doc = new XmlDocument();
            _doc.Load(System.Configuration.ConfigurationManager.AppSettings["WorkflowConfig"]);
            XmlNodeList workflows = _doc.DocumentElement.SelectNodes("Workflow");
            foreach (XmlNode _workflowNode in workflows)
            {
                Workflow workflow = new Workflow();
                workflow.ID = _workflowNode.SelectSingleNode("id").InnerText;
                workflow.FileName = _workflowNode.SelectSingleNode("filename").InnerText;
                workflow.Status = _workflowNode.SelectSingleNode("status").InnerText;
                workflow.DatePublished = _workflowNode.SelectSingleNode("date-published").InnerText;
                workflow.LastRunDate = _workflowNode.SelectSingleNode("last-run-date").InnerText;
                workflow.Message = _workflowNode.SelectSingleNode("message").InnerText;
                WorkflowsIntance.Add(workflow);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            WorkflowsIntance = null;
        }
    }
}