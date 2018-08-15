using FlowEngine.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FlowEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkflowLoader loader = new WorkflowLoader(args[0]);
            loader.setLibPath(AppSettings.get("LibPath"));
            loader.LoadActivities();
            loader.runWorkflow();
        }
    }
}
