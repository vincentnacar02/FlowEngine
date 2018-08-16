using FlowEngine.Core;
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
        private String _libPath;
        private XmlDocument _doc = new XmlDocument();
        private IDictionary<object, IActivity> _activities = new Dictionary<object, IActivity>();
        private IDictionary<object, ActivityReturn> _inMemoryActivityReturn = new Dictionary<object, ActivityReturn>();

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
                        Console.WriteLine("Assembly Loaded {0}", id);
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
                    default:
                        break;
                }
            }
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

            Console.WriteLine("executing activity [{0}]", toExecute.getId());
            IResult currentResult = toExecute.run();

            ActivityReturn activityReturn = null;
            if (activity.Attributes["return"] != null)
            {
                String returnField = activity.Attributes["return"].Value;
                String returnType = activity.Attributes["return-type"].Value;
                String resultValue = currentResult.getData()[returnField].ToString();
                if (resultValue != null)
                {
                    Console.WriteLine("activity {0} return {1} with type {2}", currentId, resultValue, returnType);
                    activityReturn = new ActivityReturn(currentId, returnField, resultValue, returnType);
                }
            }

            if (currentResult.getStatus().Equals(ResultStatus.SUCCESS))
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Error! {0}", currentResult.getException().Message);
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
                    _assertResult = new ConditionResult(expectedValue.Equals(activityReturn.ReturnValue), parseDoNodes(condition), parseElseNodes(condition));
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

    }
}
