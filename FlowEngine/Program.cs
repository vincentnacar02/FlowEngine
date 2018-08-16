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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            String worflowPath = args[0];
            log.InfoFormat("FlowEngine execution start [{0}]", worflowPath);
            WorkflowLoader loader = new WorkflowLoader(worflowPath);
            loader.setLibPath(AppSettings.get("LibPath"));
            loader.LoadActivities();
            loader.runWorkflow();
            log.InfoFormat("FlowEngine execution finished [{0}]", worflowPath);
        }
    }
}
