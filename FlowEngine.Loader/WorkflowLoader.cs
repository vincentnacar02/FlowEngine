using FlowEngine.Core;
using FlowEngine.Loader.utils;
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

namespace FlowEngine.Loader
{
    public class WorkflowLoader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private String _libPath;
        private XmlDocument _doc = new XmlDocument();
        private IDictionary<object, IActivity> _activities = new Dictionary<object, IActivity>();
        private IDictionary<object, ActivityReturn> _inMemoryActivityReturn = new Dictionary<object, ActivityReturn>();
        private IDictionary<string, object> _variableRegistry = new Dictionary<string, object>();

        public WorkflowLoader(String workflowPath)
        {
            this._doc.Load(workflowPath);
        }

        public void LoadActivities()
        {
            XmlNodeList activityNodes = this._doc.DocumentElement.SelectNodes("Activities/Activity");
            foreach (XmlNode activity in activityNodes)
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

        public void runWorkflow()
        {
            XmlNodeList execution = this._doc.DocumentElement.SelectNodes("Execution/*");
            runRecursive(execution);
        }

        private void runRecursive(XmlNodeList execution)
        {
            foreach (XmlNode line in execution)
            {
                switch (line.Name)
                {
                    case "Activity":
                        ActivityReturn result = this.executeActivity(line);
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
                        ConditionResult conditionResult = this.assertCondition(line);
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
                        this.executeForEach(line);
                        break;
                    case "Assign":
                        this.executeAssign(line);
                        break;
                    case "Variable":
                        this.createVariable(line);
                        break;
                    case "Logger":
                        this.executeLogger(line);
                        break;
                    case "Repeat":
                        this.executeRepeat(line);
                        break;
                    default:
                        break;
                }
            }
        }

        private void createVariable(XmlNode variableNode)
        {
            string name = variableNode.Attributes["name"].Value;
            object value = variableNode.Attributes["value"].Value;
            _variableRegistry.Add(name, value);
        }

        public void setLibPath(String libPath)
        {
            this._libPath = libPath;
        }

        private ActivityReturn executeActivity(XmlNode activityNode)
        {
            XmlNode activity = activityNode;

            String currentId = activity.Attributes["id"].Value;
            IActivity toExecute = _activities[currentId];

            log.DebugFormat("executing activity [{0}]", toExecute.getId());
            IResult currentResult = toExecute.run();

            ActivityReturn activityReturn = null;
            if (activity.Attributes["return"] != null)
            {
                String returnField = activity.Attributes["return"].Value;
                String returnType = activity.Attributes["return-type"].Value;
                object resultValue = currentResult.getData()[returnField];
                if (resultValue != null)
                {
                    log.DebugFormat("activity {0} return {1} with type {2}", currentId, resultValue, returnType);
                    activityReturn = new ActivityReturn(currentId, returnField, resultValue, returnType);
                }
            }

            if (currentResult.getStatus().Equals(ResultStatus.SUCCESS))
            {
                log.DebugFormat("Success >> {0}", currentId);
            }
            else
            {
                log.DebugFormat("Error! >> {0}", currentResult.getException().Message);
            }
            return activityReturn;
        }

        private ConditionResult assertCondition(XmlNode condition)
        {
            ConditionResult _assertResult = null;

            object activityIdToAssert = condition.Attributes["activityId"].Value;
            object expectedValue = condition.Attributes["value"].Value;
            string conditionType = condition.Attributes["condition"].Value;

            ActivityReturn activityReturn = this._inMemoryActivityReturn[activityIdToAssert];
            if (activityReturn == null)
            {
                throw new Exception("Could not assert [null] activity return.");
            }

            if (activityReturn.ReturnValue == null)
            {
                throw new Exception("Could not assert null return value.");
            }

            switch (conditionType)
            {
                case "EqualsTo":
                    _assertResult = new ConditionResult(AssertionUtil.equals(expectedValue, activityReturn.ReturnValue), parseDoNodes(condition), parseElseNodes(condition));
                    break;
                default:
                    break;
            }
            return _assertResult;
        }

        private XmlNodeList parseDoNodes(XmlNode node)
        {
            return node.SelectNodes("Do/*");
        }

        private XmlNodeList parseElseNodes(XmlNode node)
        {
            return node.SelectNodes("Else/*");
        }

        private void executeForEach(XmlNode forEachNode)
        {
            object activityId = forEachNode.Attributes["activityId"].Value;
            string asVariableName = forEachNode.Attributes["as"].Value;

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
                XmlNodeList doNodes = parseDoNodes(forEachNode);
                //TODO: execute recursive the activity inside this block
                log.DebugFormat("Item: {0}", item);

                runRecursive(doNodes);

                _variableRegistry.Remove(asVariableName); // remove in memory
            }
        }

        private void executeRepeat(XmlNode repeatNode)
        {
            string repeatFor = repeatNode.Attributes["times"].Value;

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
                XmlNodeList doNodes = parseDoNodes(repeatNode);
                runRecursive(doNodes);
            }
        }

        private void executeAssign(XmlNode assignNode)
        {
            string assignType = assignNode.Attributes["type"].Value;
            string assignTo = assignNode.Attributes["to"].Value;
            string assignFrom = assignNode.Attributes["from"].Value;

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
            return value;
        }

        private void executeLogger(XmlNode loggerNode)
        {
            string logType = loggerNode.Attributes["type"].Value;
            string logValue = loggerNode.Attributes["value"].Value;
            switch (logType)
            {
                case "Info":
                    log.Info(resolveAssignableValue(logValue));
                    break;
                case "Debug":
                    log.Debug(resolveAssignableValue(logValue));
                    break;
                default:
                    break;
            }
        }

    }
}
