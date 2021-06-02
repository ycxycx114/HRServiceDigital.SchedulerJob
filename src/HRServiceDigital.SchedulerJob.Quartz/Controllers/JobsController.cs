using Dapper;
using HRServiceDigital.SchedulerJob.Quartz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz.Controllers
{
    [Route("api/services/quartz/[controller]")]
    [ApiController]
    public class JobsController : SchedulerJobControllerBase
    {
        private readonly ISchedulerFactory _SchedulerFactory;
        private readonly IConfiguration _Configuration;

        public JobsController(ISchedulerFactory schedulerFactory, IConfiguration configuration)
        {
            _SchedulerFactory = schedulerFactory;
            _Configuration = configuration;
        }

        [HttpGet]
        public async Task<List<HrsJob>> GetAll()
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));

            List<HrsJob> result = new List<HrsJob>();
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            var groups = await scheduler.GetJobGroupNames();
            foreach (var group in groups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupEquals(group);
                var jobKeys = await scheduler.GetJobKeys(groupMatcher);
                foreach (var key in jobKeys)
                {
                    var jobDetail = await scheduler.GetJobDetail(key);
                    result.Add(new HrsJob
                    {
                        SchedulerName = schedulerName,
                        JobName = key.Name,
                        JobGroup = key.Group,
                        Description = jobDetail.Description,
                        IsDurable = jobDetail.Durable,
                        IsNonConcurrent = jobDetail.ConcurrentExecutionDisallowed,
                        RequestsRecovery = jobDetail.RequestsRecovery,
                        JobClassName = jobDetail.JobType.FullName + ", " + jobDetail.JobType.Assembly.GetName().Name,
                        JobData = JsonConvert.SerializeObject(jobDetail.JobDataMap)
                    });
                }
            }
            return result;
        }

        [HttpGet("{name}/{group}")]
        public async Task<HrsJob> Get([FromRoute] string name, string group)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            var jobDetail = await scheduler.GetJobDetail(new JobKey(name, group));
            if(jobDetail != null)
            {
                return new HrsJob
                {
                    SchedulerName = schedulerName,
                    JobName = name,
                    JobGroup = group,
                    Description = jobDetail.Description,
                    IsDurable = jobDetail.Durable,
                    IsNonConcurrent = jobDetail.ConcurrentExecutionDisallowed,
                    RequestsRecovery = jobDetail.RequestsRecovery,
                    JobClassName = jobDetail.JobType.FullName + ", " + jobDetail.JobType.Assembly.GetName().Name,
                    JobData = JsonConvert.SerializeObject(jobDetail.JobDataMap)
                };
            }
            return default;
        }

        [HttpPost]
        public async Task Post([FromBody] HrsJob hrsJob)
        {
            var jobName = Guid.NewGuid();
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));

            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            await scheduler.AddJob(JobBuilder.Create<WebApiJob>()
                .WithIdentity(jobName.ToString(), hrsJob.JobGroup)
                .WithDescription(hrsJob.Description)
                .UsingJobData(new JobDataMap(hrsJob.GetJobDataMap()))
                .StoreDurably()
                .Build(),
                false);
        }

        [HttpPut("{name}/{group}")]
        public async Task Put(string name, string group, [FromBody] HrsJob hrsJob)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            await scheduler.AddJob(JobBuilder.Create<WebApiJob>()
                .WithIdentity(name, group)
                .WithDescription(hrsJob.Description)
                .UsingJobData(new JobDataMap(hrsJob.GetJobDataMap()))
                .StoreDurably()
                .Build(),
                true);
        }

        [HttpDelete("{name}/{group}")]
        public async Task Delete(string name, string group)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));

            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            await scheduler.DeleteJob(new JobKey(name, group));
        }

        [Route("schedulejob")]
        [HttpPost]
        public async Task ScheduleJob([FromBody] HrsScheduleJob hrsScheduleJob)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);

            var job = JobBuilder.Create<WebApiJob>()
                .WithIdentity(Guid.NewGuid().ToString(), hrsScheduleJob.JobGroup)
                .WithDescription(hrsScheduleJob.JobDescription)
                .UsingJobData(new JobDataMap(hrsScheduleJob.GetJobDataMap()))
                .StoreDurably()
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(hrsScheduleJob.CronExpression)
                .WithDescription(hrsScheduleJob.TriggerDescription)
                .WithIdentity(Guid.NewGuid().ToString(), hrsScheduleJob.TriggerGroup)
                .ForJob(job)
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

    }
}
