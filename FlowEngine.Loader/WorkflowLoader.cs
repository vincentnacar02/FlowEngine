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
            foreach (XmlNode line in execution)
            {
                if (line.Name.Equals("Activity"))
                {
                    XmlNode activity = line;
                    object currentId = activity.Attributes["id"].Value;
                    IActivity toExecute = _activities[currentId];
                    Console.WriteLine("executing activity [{0}]", toExecute.getId());
                    IResult result1 = toExecute.run();
                    if (result1.getStatus().Equals(ResultStatus.SUCCESS))
                    {
                        Console.WriteLine("Success!");
                    }
                    else
                    {
                        Console.WriteLine("Error! {0}", result1.getException().Message);
                    }
                }
            }
        }

        public void setLibPath(String libPath)
        {
            this._libPath = libPath;
        }

    }
}
