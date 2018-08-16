using FlowEngine.Core;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileInfo
{
    public class FileInfo : Activity
    {
        public FileInfo(object Id, IProperties props)
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
                    if (isExist)
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
                        result.Add("FileName", fileInfo.Name);
                        result.Add("Directory", fileInfo.DirectoryName);
                        result.Add("Extension", fileInfo.Extension);
                        result.Add("FilePath", fileInfo.FullName);
                    }
                    result.Add("IsExist", isExist);
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(result, ResultStatus.SUCCESS);
        }
    }
}
