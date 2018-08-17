using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    public class ActivityResult : IResult
    {
        private IDictionary<string, object> _data;
        private ResultStatus _status;
        private Exception _ex;

        public ActivityResult(IDictionary<string, object> data, ResultStatus status)
        {
            this._data = data;
            this._status = status;
        }

        public ActivityResult(IDictionary<string, object> data, ResultStatus status, Exception ex)
        {
            this._data = data;
            this._status = status;
            this._ex = ex;
        }

        public IDictionary<string, object> getData()
        {
            return this._data;
        }

        public ResultStatus getStatus()
        {
            return this._status;
        }

        public Exception getException()
        {
            return this._ex;
        }
    }
}
