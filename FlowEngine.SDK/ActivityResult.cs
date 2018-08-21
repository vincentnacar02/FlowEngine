using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngine.SDK
{
    /// <summary>
    /// Result of Activity
    /// </summary>
    public class ActivityResult : IResult
    {
        private IDictionary<string, object> _data;
        private ResultStatus _status;
        private Exception _ex;

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="data">Data returned by the activity</param>
        /// <param name="status">Status</param>
        public ActivityResult(IDictionary<string, object> data, ResultStatus status)
        {
            this._data = data;
            this._status = status;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="data">Data returned by the activity</param>
        /// <param name="status">Status</param>
        /// <param name="ex">Exception if there is an error</param>
        public ActivityResult(IDictionary<string, object> data, ResultStatus status, Exception ex)
        {
            this._data = data;
            this._status = status;
            this._ex = ex;
        }

        /// <summary>
        /// Gets data returned by the activity
        /// </summary>
        /// <returns>Data</returns>
        public IDictionary<string, object> getData()
        {
            return this._data;
        }

        /// <summary>
        /// Gets status returned by the activity
        /// </summary>
        /// <returns>Status</returns>
        public ResultStatus getStatus()
        {
            return this._status;
        }

        /// <summary>
        /// Get Exception returned by the activity
        /// </summary>
        /// <returns>Exception</returns>
        public Exception getException()
        {
            return this._ex;
        }
    }
}
