using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK.interfaces
{
    public interface IProperties
    {
        void addProperty(IProperty prop);
        IProperty getProperty(string key);
        IDictionary<string, IProperty> getItems();
    }
}
