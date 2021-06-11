using Quartz;
using Quartz.Listener;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Core.Listeners
{
    public class WebApiJobListenser : JobListenerSupport
    {
        public override string Name => nameof(WebApiJobListenser);

        public override Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("*******************************job execution start.**************************");
            return base.JobToBeExecuted(context, cancellationToken);
        }

        public override Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("-----------------job executed.-------------");
            if(jobException != null)
            {
                Console.WriteLine(jobException.Message);
            }
            return base.JobWasExecuted(context, jobException, cancellationToken);
        }
    }
}
