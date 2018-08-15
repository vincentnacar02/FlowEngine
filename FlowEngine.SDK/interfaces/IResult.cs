using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK.interfaces
{
    public interface IResult
    {
        IDictionary<string, object> getData();

        ResultStatus getStatus();

        Exception getException();
    }
}
