using FlowEngine.SDK;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExist
{
    public class FileExist : Activity
    {
        public FileExist(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                var filePathProp = this.getProperties().getProperty("FilePath");
                if (filePathProp != null)
                {
                    String filePath = (String)filePathProp.getValue();
                    Boolean isExist = File.Exists(filePath);
                    result.Add("IsExist", isExist);
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(result, ResultStatus.SUCCESS);
        }

        public override void onInit()
        {

        }
    }
}
