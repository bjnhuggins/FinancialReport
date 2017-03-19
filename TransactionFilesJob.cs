using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialReport
{
    public class TransactionFilesJob : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(ProcessFile.workingDirectory + ProcessFile.pending);
            // Get files in directory and order by write time, assuming this is the same time as in the filename
            FileInfo[] files = directoryInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f.LastWriteTime).ToArray();

            // Process each file found in the list. 
            // TODO: make this use parallel processing.
            foreach (FileInfo file in files)
            {
                if (File.Exists(file.FullName))
                {
                    ProcessFile.readFile(file.FullName, file.Name);
                }
            }
        }
    }
}
