using FlowEngine.Core.elements.interfaces;
using FlowEngine.Core.providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Core
{
    /// <summary>
    /// Workflow
    /// @author: VincentNacar
    /// </summary>
    public class Workflow : IWorkflow
    {
        private XmlNodeList SettingsNode;
        private XmlNodeList ActivitiesNode;
        private XmlNodeList ExecutionNode;

        private IWorkflowState StateProvider;
        private IList<IElement> ExecutionElements;

        public Workflow()
        {

        }

        public Workflow(XmlNodeList settingsNode, XmlNodeList activitiesNode, XmlNodeList executionNode)
        {
            this.SettingsNode = settingsNode;
            this.ActivitiesNode = activitiesNode;
            this.ExecutionNode = executionNode;

            this.ExecutionElements = new List<IElement>();
        }

        public void InitializeElements()
        {
            // initialize only the execution block
            InitRecursive(this.ExecutionNode);
        }

        public void SetState(IWorkflowState stateProvider)
        {
            this.StateProvider = stateProvider;
        }

        public IWorkflowState GetState()
        {
            return this.StateProvider;
        }

        private void InitRecursive(XmlNodeList nodes)
        {
            foreach (XmlNode line in nodes)
            {
                IElement lineElement = ElementFactory.CreateElement(line);
                this.ExecutionElements.Add(lineElement);
            }
        }

        public XmlNodeList getSettingsNode()
        {
            return this.SettingsNode;
        }

        public XmlNodeList getActivitiesNode()
        {
            return this.ActivitiesNode;
        }

        public XmlNodeList getExecutionNode()
        {
            return this.ExecutionNode;
        }

        public IList<IElement> getExecutionElements()
        {
            return this.ExecutionElements;
        }
    }
}
