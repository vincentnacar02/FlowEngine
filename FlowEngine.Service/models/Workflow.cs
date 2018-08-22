using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlowEngine.Service.models
{
    public class Workflow
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string DatePublished { get; set; }
        public string LastRunDate { get; set; }
        public string Message { get; set; }
    }
}