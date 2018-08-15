using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine
{
    class AppSettings
    {
        public static string get(String key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
