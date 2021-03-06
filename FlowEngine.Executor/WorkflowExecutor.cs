﻿using FlowEngine.Core;
using FlowEngine.Core.activity;
using FlowEngine.Core.constants;
using FlowEngine.Core.container;
using FlowEngine.Core.control_flow;
using FlowEngine.Core.elements.interfaces;
using FlowEngine.Core.input;
using FlowEngine.Core.output;
using FlowEngine.Core.providers;
using FlowEngine.Executor.types;
using FlowEngine.Executor.utils;
using FlowEngine.SDK;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.providers;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine.Executor
{
    public class WorkflowExecutor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("workflow-logger");

        private String _libPath;
        private XmlDocument _doc = new XmlDocument();

        private _AttributeSelectorImpl _AttributeSelector;

        private Workflow _workflow;
        private WorkflowStateProvider _stateProvider;
        private SettingsProvider _settingsProvider;

        public WorkflowExecutor(String workflowPath)
        {
            this._doc.Load(workflowPath);
        }

        //TODO: need to refactor
        public void InitializeWorkflow()
        {
            try
            {
                log.Debug("Initialize workflow");
                XmlNodeList settingsNode = this._doc.DocumentElement.SelectNodes("Settings/*");
                XmlNodeList activitiesNode = this._doc.DocumentElement.SelectNodes("Activities/Activity");
                XmlNodeList executionNode = this._doc.DocumentElement.SelectNodes("Execution/*");
                this._workflow = new Workflow(settingsNode, activitiesNode, executionNode);

                this._stateProvider = new WorkflowStateProvider();
                this._workflow.SetState(this._stateProvider);

                this.InitializeGlobalSettings();

                this.InitializeActivities();

                this._workflow.InitializeElements();

                this._AttributeSelector = new _AttributeSelectorImpl(this._stateProvider.getVariables(), this._stateProvider.getActivityResults());
            }
            catch (Exception ex)
            {
                log.DebugFormat("Workflow Error: {0}", ex.Message);
                throw ex;
            }
        }

        private void InitializeGlobalSettings()
        {
            IDictionary<string, object> _settings = new Dictionary<string, object>();
            foreach (XmlNode setting in this._workflow.getSettingsNode())
            {
                string name = setting.Attributes[AttributeConstants.NAME].Value;
                object value = setting.Attributes[AttributeConstants.VALUE].Value;
                if (_settings.ContainsKey(name))
                {
                    throw new Exception(String.Format("Duplicate key is not allowed in global settings. Key: {0}", name));
                }
                log.DebugFormat("Adding setting [{0}] with value: {1}", name, value);
                _settings.Add(name, value);
            }
            this._settingsProvider = new SettingsProvider(_settings);
        }

        private void InitializeActivities()
        {
            foreach (XmlNode activity in this._workflow.getActivitiesNode())
            {
                if (activity.Attributes[AttributeConstants.ACTIVITY_ASSEMBLY].Value != null)
                {
                    String id = activity.Attributes[AttributeConstants.ID].Value;
                    XmlNodeList activityProps = activity.SelectNodes("Property");
                    Properties props = new Properties();
                    foreach (XmlNode prop in activityProps)
                    {
                        props.addProperty(new Property(prop.Attributes[AttributeConstants.NAME].Value, prop.Attributes[AttributeConstants.VALUE].Value));
                    }

                    String libName = activity.Attributes[AttributeConstants.ACTIVITY_ASSEMBLY].Value;
                    String assemblyPath = Path.Combine(this._libPath, libName);
                    log.DebugFormat("Loading assembly {0}", assemblyPath);
                    var DLL = Assembly.LoadFile(assemblyPath);
                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        dynamic activity_instance = Activator.CreateInstance(type, id, props);

                        activity_instance.onInit();

                        activity_instance.onInitSettings(this._settingsProvider);

                        this._stateProvider.getActivities().Add(id, activity_instance);
                        log.DebugFormat("Assembly Loaded {0}", id);
                    }

                }
            }
        }

        public void RunWorkflow()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                runRecursive(this._workflow.getExecutionElements());
            
                watch.Stop();

                log.DebugFormat("Workflow execution finished. Elapsed Time : {0}", watch.Elapsed);
            }
            catch (Exception ex)
            {
                log.DebugFormat("Workflow Error: {0}", ex.Message);
                throw ex;
            }
        }

        private void runRecursive(IList<IElement> execution)
        {
            foreach (IElement line in execution)
            {
                switch (line.getElementName())
                {
                    case ElementNameConstants.ACTIVITY:
                        var activity = (ActivityElement) line;
                        IResult result = this.executeActivity(activity);
                        if (result != null)
                        {
                            if (this._stateProvider.getActivityResults().ContainsKey(activity.getAttribute(AttributeConstants.ID).getValue()))
                            {
                                this._stateProvider.getActivityResults().Remove(activity.getAttribute(AttributeConstants.ID).getValue());
                            }
                            this._stateProvider.getActivityResults().Add(activity.getAttribute(AttributeConstants.ID).getValue(), result);
                        }
                        break;
                    case ElementNameConstants.IF:
                        ConditionResult conditionResult = this.assertCondition((IfElement) line);
                        if (conditionResult.Result)
                        {
                            if (conditionResult.DoNodes != null)
                            {
                                runRecursive(conditionResult.DoNodes);
                            }
                        }
                        else
                        {
                            if (conditionResult.ElseNodes != null)
                            {
                                runRecursive(conditionResult.ElseNodes);
                            }
                        }
                        break;
                    case ElementNameConstants.FOR_EACH:
                        this.executeForEach((ForEachElement) line);
                        break;
                    case ElementNameConstants.ASSIGN:
                        this.executeAssign((AssignElement) line);
                        break;
                    case ElementNameConstants.VARIABLE:
                        this.createVariable((VariableElement) line);
                        break;
                    case ElementNameConstants.LOGGER:
                        this.executeLogger((LoggerElement) line);
                        break;
                    case ElementNameConstants.REPEAT:
                        this.executeRepeat((RepeatElement) line);
                        break;
                    default:
                        break;
                }
            }
        }

        private void createVariable(VariableElement variableNode)
        {
            object name = variableNode.getAttribute(AttributeConstants.NAME).getValue();
            object value = variableNode.getAttribute(AttributeConstants.VALUE).getValue();
            this._stateProvider.getVariables().Add(name, value);
        }

        public void setLibPath(String libPath)
        {
            this._libPath = libPath;
        }

        private IResult executeActivity(ActivityElement activityNode)
        {
            String currentId = activityNode.getAttribute(AttributeConstants.ID).getValue().ToString();
            IActivity toExecute = this._stateProvider.getActivities()[currentId];

            log.DebugFormat("executing activity [{0}]", toExecute.getId());
            IResult currentResult = toExecute.run();

            if (currentResult.getStatus().Equals(ResultStatus.SUCCESS))
            {
                Int32 resultCount = 0;
                if (currentResult.getData() != null)
                    resultCount = currentResult.getData().Count;

                log.DebugFormat("Execute Activity success >> {0}, Result Data count: {1}", currentId, resultCount);
            }
            else
            {
                log.DebugFormat("Execute Activity[{0}] error >> {1}", currentId, currentResult.getException().Message);
            }
            return currentResult;
        }

        private ConditionResult assertCondition(IfElement condition)
        {
            ConditionResult _assertResult = null;

            string valueOF = condition.getAttribute(AttributeConstants.VALUE_OF).getValue().ToString();
            object expectedValue = condition.getAttribute(AttributeConstants.VALUE).getValue();
            string conditionType = condition.getAttribute(AttributeConstants.CONDITION).getValue().ToString();

            object valueToCheck = this._AttributeSelector.valueOf(valueOF);
            if (valueToCheck == null)
            {
                throw new Exception("Could not assert [null] value-of.");
            }

            try
            {
                ConditionType _condition = EnumUtil.Parse<ConditionType>(conditionType);
                _assertResult = new ConditionResult(
                        AssertionUtil.AssertCondition(_condition, expectedValue, valueToCheck),
                        condition.DoNodes,
                        condition.ElseNodes);
            }
            catch (Exception ex)
            { 
                throw ex;
            }
            return _assertResult;
        }

        // TODO: test
        private void executeWhile(WhileElement whileNode)
        {
            string valueOF = whileNode.getAttribute(AttributeConstants.VALUE_OF).getValue().ToString();
            object expectedValue = whileNode.getAttribute(AttributeConstants.VALUE).getValue();
            string conditionType = whileNode.getAttribute(AttributeConstants.CONDITION).getValue().ToString();

            object valueToCheck = this._AttributeSelector.valueOf(valueOF);
            if (valueToCheck == null)
            {
                throw new Exception("Could not assert [null] value-of.");
            }

            try
            {
                ConditionType _condition = EnumUtil.Parse<ConditionType>(conditionType);
                while (AssertionUtil.AssertCondition(_condition, expectedValue, valueToCheck))
                {
                    if (whileNode.hasDoNodes())
                    {
                        runRecursive(whileNode.DoNodes);
                    }
                }
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }

        private void executeForEach(ForEachElement forEachNode)
        {
            string valueOf = forEachNode.getAttribute(AttributeConstants.VALUE_OF).getValue().ToString();
            string asVariableName = forEachNode.getAttribute(AttributeConstants.AS).getValue().ToString();

            object selectedValue = this._AttributeSelector.valueOf(valueOf);
            if (selectedValue == null)
            {
                throw new Exception("Could not do ForEach, [null] activity return.");
            }

            if (selectedValue.GetType() != typeof(List<string>))
            {
                throw new Exception("Could not do ForEach, expected return-type 'List'.");
            }

            IList<string> list = (IList<string>) selectedValue;
            foreach (var item in list)
            {
                this._stateProvider.getVariables().Add(asVariableName, item);
                IList<IElement> doNodes = forEachNode.DoNodes;

                runRecursive(doNodes);

                this._stateProvider.getVariables().Remove(asVariableName); // remove in memory
            }
        }

        private void executeRepeat(RepeatElement repeatNode)
        {
            string repeatFor = repeatNode.getAttribute(AttributeConstants.TIMES).getValue().ToString();

            Int32 repeatTimes = 0;
            try
            {
                Int32.TryParse(repeatFor, out repeatTimes);
            }
            catch (Exception)
            {
                throw new Exception("repeat time must be a number!");
            }
            
            for (int i = 0; i < repeatTimes; i++)
            {
                IList<IElement> doNodes = repeatNode.DoNodes;
                runRecursive(doNodes);
            }
        }

        private void executeAssign(AssignElement assignNode)
        {
            string assignType = assignNode.getAttribute(AttributeConstants.TYPE).getValue().ToString();
            string assignTo = assignNode.getAttribute(AttributeConstants.TO).getValue().ToString();
            string assignFrom = assignNode.getAttribute(AttributeConstants.FROM).getValue().ToString(); //TODO: use value-of here

            switch (assignType)
            {
                case "Property":
                    String[] activityProperty = assignTo.Split('.');
                    if (activityProperty != null && activityProperty[0] != null && activityProperty[1] != null)
                    {
                        IActivity activity = this._stateProvider.getActivities()[activityProperty[0]];
                        if (activity != null)
                        {
                            activity.setPropertyValue(activityProperty[1], this._AttributeSelector.valueOf(assignFrom));
                        }
                    }
                    break;
                case "Variable":
                    this._stateProvider.getVariables()[assignTo] = this._AttributeSelector.valueOf(assignFrom);
                    break;
                default:
                    break;
            }
        }

        private void executeLogger(LoggerElement loggerNode)
        {
            string logType = loggerNode.getAttribute(AttributeConstants.TYPE).getValue().ToString();
            string logValue = loggerNode.getAttribute(AttributeConstants.VALUE).getValue().ToString();
            switch (logType)
            {
                case "Info":
                    logger.Info(this._AttributeSelector.valueOf(logValue));
                    break;
                case "Debug":
                    logger.Debug(this._AttributeSelector.valueOf(logValue));
                    break;
                default:
                    break;
            }
        }

        class _AttributeSelectorImpl : AttributeSelector
        {
            private IDictionary<object, object> _VariableInstances;
            private IDictionary<object, IResult> _ActivityInstances;

            public _AttributeSelectorImpl(IDictionary<object, object> variableInstances, IDictionary<object, IResult> activityInstances)
            {
                this._VariableInstances = variableInstances;
                this._ActivityInstances = activityInstances;
            }

            protected override IDictionary<object, object> getVariableRegistry()
            {
                return this._VariableInstances;
            }

            protected override IDictionary<object, IResult> getActivityInstances()
            {
                return this._ActivityInstances;
            }
        }

    }
}
