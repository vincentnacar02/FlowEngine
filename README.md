# FlowEngine
FlowEngine v1

This is a hobby project only!

### Example workflow:
This example is getting a list of files in a directory and logging each filename.
``` xml
<Workflow id="ABC3C391-B1FE-416E-A606-B2C42BD49EB9"
	name="demo workflow 2" flow-engine-version="1.0"
	author="Vincent Nacar" date-published="11/02/2019">

  <!-- Activity -->
  <Activities>
	
	<Activity id="ListFiles1" name="ListFiles" assembly="Activity.ListFiles.dll">
	  <Property name="DirectoryPath" value="C:\VINCE FILES\Source Codes\flow-project\FlowEngine\FlowEngine\bin\Debug\Workflows\Workflow2TestDir\Input" />
	</Activity>
	
	<Activity id="FileInfoCheck" name="FileInfo" assembly="Activity.FileInfo.dll">
	  <Property name="FilePath" value="" />
	</Activity>

  </Activities>

  <!-- Execution flow -->
  <Execution>
	<!-- variable declaration -->
	<Variable name="OutputPathWithFileName" type="String" value="" />
	
	<Activity id="ListFiles1" return="Files" return-type="List" />
	<ForEach activityId="ListFiles1" as="File" >
		<Do>
			<Assign type="Property" to="FileInfoCheck.FilePath" from="@File@" />
			<Activity id="FileInfoCheck" return="FileName" return-type="String" />
			
			<Assign type="Variable" to="OutputPathWithFileName" from="[FileInfoCheck]" />
			
			<Logger type="Info" value="[FileInfoCheck]" />
			<Logger type="Info" value="@OutputPathWithFileName@" />
		</Do>
	</ForEach>
  </Execution>
  
</Workflow>
```

### Activity code sample
This is the code behind FileInfo activity.
``` c#
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
```

# Control Flow Elements
* ## If/Else
* ## ForEach
* ## Repeat
* ## While
* ## Switch
* ## Do
# Input Elements
* ## Assign
# Output Elements
* ## Logger
# Container
* ## Variable
# Activities
* ## File System Activities
  * CopyFile
  * DeleteFile
  * FileExist
  * FileInfo
  * ListFiles
  * MoveFile
