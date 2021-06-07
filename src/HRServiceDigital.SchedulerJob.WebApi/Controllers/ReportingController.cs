using Dapper;
using HRServiceDigital.SchedulerJob.Core.Utils;
using HRServiceDigital.SchedulerJob.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    [Route("api/services/quartz/[controller]")]
    [ApiController]
    public class ReportingController : SchedulerJobControllerBase
    {

        private readonly IDbConnection _DbConnection;
        private readonly ISchedulerFactory _SchedulerFactory;

        public ReportingController(IDbConnection dbConnection,
            ISchedulerFactory schedulerFactory)
        {
            _DbConnection = dbConnection;
            _SchedulerFactory = schedulerFactory;
        }

        [HttpGet]
        [Route("allJobs")]
        public async Task<IEnumerable<JobViewModel>> GetJobs(string schedulerName = "QuartzScheduler")
        {
            string sql = @"SELECT
                             SCHED_NAME
                               ,JOB_NAME
                               ,JOB_GROUP
                               ,DESCRIPTION
                               ,JOB_CLASS_NAME
                               ,IS_DURABLE
                               ,IS_NONCONCURRENT
                               ,IS_UPDATE_DATA
                               ,REQUESTS_RECOVERY
                               ,JOB_DATA
                            FROM QRTZ_JOB_DETAILS
                            WHERE SCHED_NAME = @Scheduler";

            return await _DbConnection.QueryAsync<JobViewModel>(sql, new { Scheduler = new DbString { Value = schedulerName } });
        }

        [HttpGet]
        [Route("allTriggers")]
        public async Task<IEnumerable<TriggerViewModel>> GetTrigger(string schedulerName = "QuartzScheduler")
        {
            string sql = @"SELECT 
                            t0.SCHED_NAME,
                            t0.TRIGGER_NAME,
                            t0.TRIGGER_GROUP,
                            t0.JOB_NAME, 
                            t0.JOB_GROUP,
                            t0.DESCRIPTION,
                            t0.TRIGGER_STATE,
                            t0.TRIGGER_TYPE,
                            t1.CRON_EXPRESSION,
                            t1.TIME_ZONE_ID
                            FROM QRTZ_TRIGGERS t0
                            JOIN QRTZ_CRON_TRIGGERS t1 on t0.SCHED_NAME = t1.SCHED_NAME
                            AND t0.TRIGGER_NAME = t1.TRIGGER_NAME
                            AND t0.TRIGGER_GROUP = t1.TRIGGER_GROUP
                            WHERE t0.SCHED_NAME = @Scheduler";

            return await _DbConnection.QueryAsync<TriggerViewModel>(sql,
                new { Scheduler = new DbString { Value = schedulerName } });
        }

        [HttpGet]
        [Route("triggerStatus")]
        public async Task<List<string>> GetTriggerStatus()
        {
            List<string> result = new List<string>();

            var scheduler = await _SchedulerFactory.GetScheduler(SchedulerName);

            var allTriggerKeys = await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            foreach (var key in allTriggerKeys)
            {
                var triggerStatus = await scheduler.GetTriggerState(key);
                result.Add(triggerStatus.ToString());
            }

            return result;
        }

        [HttpGet]
        [Route("runningJobs")]
        public async Task<List<JobViewModel>> GetCurrentExecutingJobs()
        {
            List<JobViewModel> result = new List<JobViewModel>();

            var scheduler = await _SchedulerFactory.GetScheduler(SchedulerName);

            var allRunningJobs = await scheduler.GetCurrentlyExecutingJobs();
            foreach (var job in allRunningJobs)
            {
                result.Add(new JobViewModel
                {
                    SchedulerName = job.Scheduler.SchedulerName,
                    JobName = job.JobDetail.Key.Name,
                    JobGroup = job.JobDetail.Key.Group,
                    Description = job.JobDetail.Description,
                    FireInstanceId = job.Scheduler.SchedulerInstanceId,
                    ScheduledFireTime = job.ScheduledFireTimeUtc.FormatDateTime(),
                    FireTime = job.FireTimeUtc.FormatDateTime(),
                    PreviousFireTimeUtc = job.PreviousFireTimeUtc.FormatDateTime(),
                    NextFireTimeUtc = job.NextFireTimeUtc.FormatDateTime(),
                    JobData = JsonConvert.SerializeObject(job.JobDetail.JobDataMap)
                });
            }
            return result;
        }
    }
}
