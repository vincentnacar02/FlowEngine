using FlowEngine.Core;
using FlowEngine.SDK.interfaces;
using FlowEngine.SDK.types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListFiles
{
    public class ListFiles : Activity
    {
        public ListFiles(object Id, IProperties props)
            : base(Id, props)
        {

        }

        public override IResult run()
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                var dirPathProp = this.getProperties().getProperty("DirectoryPath");
                if (dirPathProp != null)
                {
                    String dirPath = (String) dirPathProp.getValue();
                    String[] files = Directory.GetFiles(dirPath);

                    IList<string> filePaths = new List<string>();
                    foreach (var file in files)
                    {
                        filePaths.Add(file);
                    }

                    result.Add("Files", filePaths);
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
