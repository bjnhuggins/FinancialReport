using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace FinancialReport
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessFile.workingDirectory = args[0] + @"\";
            
            Console.Out.WriteLine("Using dir: " + ProcessFile.workingDirectory);

            // Create folders if they don't exist
            Directory.CreateDirectory(ProcessFile.workingDirectory + ProcessFile.processed);
            Directory.CreateDirectory(ProcessFile.workingDirectory + ProcessFile.pending);
            Directory.CreateDirectory(ProcessFile.workingDirectory + ProcessFile.reports);
            
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();

            RunCustomerTransactionProcess(scheduler);

            Console.Out.WriteLine("Press q followed by enter to quit");
            while (Console.Read() != 'q') ;
            scheduler.Shutdown();
        }

        private static void RunCustomerTransactionProcess(IScheduler scheduler)
        {
            try
            {
                scheduler.Start();
                
                // define the job to be run at trigger times
                IJobDetail job = JobBuilder.Create<TransactionFilesJob>()
                    .WithIdentity("transactionFilesJob", "group1")
                    .Build();

                // Create the 6am and 9pm triggers
                ITrigger morningTrigger = TriggerBuilder.Create()
                    .WithIdentity("morningTrigger", "group1")
                    .StartNow()
                    .WithCronSchedule("0 0 6 * * ?")
                    .Build();

                ITrigger afternoonTrigger = TriggerBuilder.Create()
                    .WithIdentity("afternoonTrigger", "group1")
                    .StartNow()
                    .WithCronSchedule("0 0 21 * * ?")
                    .Build();
                
                var triggersAndJobs = new Dictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>>();
                triggersAndJobs.Add(job, new Quartz.Collection.HashSet<ITrigger>()
                {
                    morningTrigger,
                    afternoonTrigger
                });

                scheduler.ScheduleJobs(triggersAndJobs, true);
            }
            catch (SchedulerException se)
            {
                // Write to a log file in future.
                Console.WriteLine(se);
            }
        }
    }
}
