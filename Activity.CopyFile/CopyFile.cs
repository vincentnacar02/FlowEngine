using FlowEngine.Core;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFile
{
    public class CopyFile : Activity
    {
        public CopyFile(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            try
            {
                var inputPathProp = this.getProperties().getProperty("InputPath");
                var destinationPathProp = this.getProperties().getProperty("DestinationPath");
                if (inputPathProp != null && destinationPathProp != null)
                {
                    String inputPath = (String)inputPathProp.getValue();
                    String destinationPath = (String)destinationPathProp.getValue();
                    File.Copy(inputPath, destinationPath);
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
