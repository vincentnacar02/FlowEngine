using FlowEngine.Executor;
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

        enum ExitCode : int
        {
            SUCCESS = 0,
            ERROR = 1
        }

        static int Main(string[] args)
        {
            try
            {
                String worflowPath = args[0];
                log.InfoFormat("FlowEngine loading start [{0}]", worflowPath);
                WorkflowExecutor executor = new WorkflowExecutor(worflowPath);
                executor.setLibPath(AppSettings.get("LibPath"));
                executor.InitializeWorkflow();
                executor.RunWorkflow();
                log.InfoFormat("FlowEngine execution finished [{0}]", worflowPath);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return (int) ExitCode.ERROR;
            }
            return (int) ExitCode.SUCCESS;
        }
    }
}
