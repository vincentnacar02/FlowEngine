using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.Core.elements.interfaces
{
    public interface IAttribute
    {
        string getName();
        object getValue();
    }
}
