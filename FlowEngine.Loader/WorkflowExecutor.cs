using FlowEngine.Core;
using FlowEngine.Core.activity;
using FlowEngine.Core.container;
using FlowEngine.Core.control_flow;
using FlowEngine.Core.elements.interfaces;
using FlowEngine.Core.input;
using FlowEngine.Core.output;
using FlowEngine.Executor.utils;
using FlowEngine.SDK;
using FlowEngine.SDK.interfaces;
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
        private IDictionary<object, IActivity> _activities = new Dictionary<object, IActivity>();
        private IDictionary<object, ActivityReturn> _inMemoryActivityReturn = new Dictionary<object, ActivityReturn>();
        private IDictionary<object, object> _variableRegistry = new Dictionary<object, object>();

        private Workflow _workflow;

        public WorkflowExecutor(String workflowPath)
        {
            this._doc.Load(workflowPath);
        }

        public void InitializeWorkflow()
        {
            log.Debug("Initialize workflow");
            XmlNodeList settingsNode = this._doc.DocumentElement.SelectNodes("Settings/*");
            XmlNodeList activitiesNode = this._doc.DocumentElement.SelectNodes("Activities/Activity");
            XmlNodeList executionNode = this._doc.DocumentElement.SelectNodes("Execution/*");
            this._workflow = new Workflow(settingsNode, activitiesNode, executionNode);

            this.InitializeActivitiesBlock();

            this._workflow.InitializeElements();
        }

        private void InitializeActivitiesBlock()
        {
            foreach (XmlNode activity in this._workflow.getActivitiesNode())
            {
                if (activity.Attributes["assembly"].Value != null)
                {
                    String id = activity.Attributes["id"].Value;
                    XmlNodeList activityProps = activity.SelectNodes("Property");
                    Properties props = new Properties();
                    foreach (XmlNode prop in activityProps)
                    {
                        props.addProperty(new Property(prop.Attributes["name"].Value, prop.Attributes["value"].Value));
                    }

                    String libName = activity.Attributes["assembly"].Value;
                    String assemblyPath = Path.Combine(this._libPath, libName);
                    Console.WriteLine("Loading assembly {0}", assemblyPath);
                    var DLL = Assembly.LoadFile(assemblyPath);
                    foreach (Type type in DLL.GetExportedTypes())
                    {
                        dynamic activity_instance = Activator.CreateInstance(type, id, props);
                        _activities.Add(id, activity_instance);
                        log.DebugFormat("Assembly Loaded {0}", id);
                    }

                }
            }
        }

        public void RunWorkflow()
        {
            runRecursive(this._workflow.getExecutionElements());
        }

        private void runRecursive(IList<IElement> execution)
        {
            foreach (IElement line in execution)
            {
                switch (line.getElementName())
                {
                    case "Activity":
                        ActivityReturn result = this.executeActivity((ActivityElement) line);
                        if (result != null)
                        {
                            if (this._inMemoryActivityReturn.ContainsKey(result.ActivityId))
                            {
                                this._inMemoryActivityReturn.Remove(result.ActivityId);
                            }
                            this._inMemoryActivityReturn.Add(result.ActivityId, result);
                        }
                        break;
                    case "If":
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
                    case "ForEach":
                        this.executeForEach((ForEachElement) line);
                        break;
                    case "Assign":
                        this.executeAssign((AssignElement) line);
                        break;
                    case "Variable":
                        this.createVariable((VariableElement) line);
                        break;
                    case "Logger":
                        this.executeLogger((LoggerElement) line);
                        break;
                    case "Repeat":
                        this.executeRepeat((RepeatElement) line);
                        break;
                    default:
                        break;
                }
            }
        }

        private void createVariable(VariableElement variableNode)
        {
            object name = variableNode.getAttribute("name").getValue();
            object value = variableNode.getAttribute("value").getValue();
            _variableRegistry.Add(name, value);
        }

        public void setLibPath(String libPath)
        {
            this._libPath = libPath;
        }

        private ActivityReturn executeActivity(ActivityElement activityNode)
        {
            String currentId = activityNode.getAttribute("id").getValue().ToString();
            IActivity toExecute = _activities[currentId];

            log.DebugFormat("executing activity [{0}]", toExecute.getId());
            IResult currentResult = toExecute.run();

            ActivityReturn activityReturn = null;
            if (activityNode.getAttribute("return") != null)
            {
                String returnField = activityNode.getAttribute("return").getValue().ToString();
                String returnType = activityNode.getAttribute("return-type").getValue().ToString();
                object resultValue = currentResult.getData()[returnField];
                if (resultValue != null)
                {
                    log.DebugFormat("activity {0} return {1} with type {2}", currentId, resultValue, returnType);
                    activityReturn = new ActivityReturn(currentId, returnField, resultValue, returnType);
                }
            }

            if (currentResult.getStatus().Equals(ResultStatus.SUCCESS))
            {
                log.DebugFormat("Execute Activity success >> {0}", currentId);
            }
            else
            {
                log.DebugFormat("Execute Activity[{0}] error >> {1}", currentId, currentResult.getException().Message);
            }
            return activityReturn;
        }

        private ConditionResult assertCondition(IfElement condition)
        {
            ConditionResult _assertResult = null;

            string valueOF = condition.getAttribute("value-of").getValue().ToString();
            object expectedValue = condition.getAttribute("value").getValue();
            string conditionType = condition.getAttribute("condition").getValue().ToString();

            object valueToCheck = resolveAssignableValue(valueOF);
            if (valueToCheck == null)
            {
                throw new Exception("Could not assert [null] value-of.");
            }

            switch (conditionType)
            {
                case "EqualsTo":
                    _assertResult = new ConditionResult(AssertionUtil.equals(expectedValue, valueToCheck), condition.DoNodes, condition.ElseNodes);
                    break;
                default:
                    break;
            }
            return _assertResult;
        }

        private void executeForEach(ForEachElement forEachNode)
        {
            object activityId = forEachNode.getAttribute("activityId").getValue();
            string asVariableName = forEachNode.getAttribute("as").getValue().ToString();

            ActivityReturn activityReturn = this._inMemoryActivityReturn[activityId];
            if (activityReturn == null)
            {
                throw new Exception("Could not do ForEach, [null] activity return.");
            }

            if (activityReturn.ReturnValue == null)
            {
                throw new Exception("Could not do ForEach, null return value.");
            }

            if (!activityReturn.ReturnType.Equals("List"))
            {
                throw new Exception("Could not do ForEach, expected return-type 'List'.");
            }

            IList<string> list = (IList<string>) activityReturn.ReturnValue;
            foreach (var item in list)
            {
                _variableRegistry.Add(asVariableName, item);
                IList<IElement> doNodes = forEachNode.DoNodes;
                //TODO: execute recursive the activity inside this block
                log.DebugFormat("Item: {0}", item);

                runRecursive(doNodes);

                _variableRegistry.Remove(asVariableName); // remove in memory
            }
        }

        private void executeRepeat(RepeatElement repeatNode)
        {
            string repeatFor = repeatNode.getAttribute("times").getValue().ToString();

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
            string assignType = assignNode.getAttribute("type").getValue().ToString();
            string assignTo = assignNode.getAttribute("to").getValue().ToString();
            string assignFrom = assignNode.getAttribute("from").getValue().ToString();

            switch (assignType)
            {
                case "Property":
                    String[] activityProperty = assignTo.Split('.');
                    if (activityProperty != null && activityProperty[0] != null && activityProperty[1] != null)
                    {
                        IActivity activity = _activities[activityProperty[0]];
                        if (activity != null)
                        {
                            activity.setPropertyValue(activityProperty[1], resolveAssignableValue(assignFrom));
                        }
                    }
                    break;
                case "Variable":
                    _variableRegistry[assignTo] = resolveAssignableValue(assignFrom);
                    break;
                default:
                    break;
            }
        }

        private object resolveAssignableValue(string assignFrom)
        {
            object value = null;
            if (assignFrom.StartsWith("@") && assignFrom.EndsWith("@"))
            {
                string variableKey = assignFrom.Replace("@", "");
                value = _variableRegistry[variableKey];
            }
            else if (assignFrom.StartsWith("[") && assignFrom.EndsWith("]"))
            {
                string activityId = assignFrom.Replace("[", "").Replace("]", "");
                ActivityReturn activityReturn = _inMemoryActivityReturn[activityId];
                value = activityReturn.ReturnValue;
            }
            else
            {
                value = assignFrom;
            }
            return value;
        }

        private void executeLogger(LoggerElement loggerNode)
        {
            string logType = loggerNode.getAttribute("type").getValue().ToString();
            string logValue = loggerNode.getAttribute("value").getValue().ToString();
            switch (logType)
            {
                case "Info":
                    logger.Info(resolveAssignableValue(logValue));
                    break;
                case "Debug":
                    logger.Debug(resolveAssignableValue(logValue));
                    break;
                default:
                    break;
            }
        }

    }
}
