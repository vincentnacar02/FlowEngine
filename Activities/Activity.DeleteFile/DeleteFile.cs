using FlowEngine.SDK;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteFile
{
    public class DeleteFile : Activity
    {
        public DeleteFile(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
                var filePathProp = this.getProperties().getProperty("FilePath");
                if (filePathProp != null)
                {
                    String filePath = (String) filePathProp.getValue();
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                return new ActivityResult(null, ResultStatus.HAS_ERROR, ex);
            }
            return new ActivityResult(null, ResultStatus.SUCCESS);
        }
    }
}
