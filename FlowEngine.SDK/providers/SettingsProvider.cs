using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK.providers
{
    public class SettingsProvider
    {
        private IDictionary<string, object> Settings;

        public SettingsProvider(IDictionary<string, object> settings)
        {
            this.Settings = settings;
        }

        public IDictionary<string, object> getSettings()
        {
            return this.Settings;
        }
    }
}
