using HRServiceDigital.SchedulerJob.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.WebApi.Controllers
{
    [Route("api/services/quartz/[controller]")]
    [ApiController]
    public class TriggersController : SchedulerJobControllerBase
    {
        private readonly ISchedulerFactory _SchedulerFactory;
        private readonly IConfiguration _Configuration;

        public TriggersController(ISchedulerFactory schedulerFactory, IConfiguration configuration)
        {
            _SchedulerFactory = schedulerFactory;
            _Configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<HrsTrigger>> GetAll()
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            List<HrsTrigger> result = new List<HrsTrigger>();
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            var groups = await scheduler.GetTriggerGroupNames();
            foreach (var group in groups)
            {
                var groupMatcher = GroupMatcher<TriggerKey>.GroupEquals(group);
                var triggerKeys = await scheduler.GetTriggerKeys(groupMatcher);
                foreach (var key in triggerKeys)
                {
                    var trigger = await scheduler.GetTrigger(key);
                    var type = trigger.GetType();
                    ICronTrigger cronTrigger = trigger as ICronTrigger;
                    result.Add(new HrsTrigger
                    {
                        SchedulerName = schedulerName,
                        TriggerName = key.Name,
                        TriggerGroup = key.Group,
                        JobName = trigger.JobKey.Name,
                        JobGroup = trigger.JobKey.Group,
                        Description = trigger.Description,
                        CronExpression = cronTrigger.CronExpressionString,
                        TimeZoneId = cronTrigger.TimeZone.Id
                    });
                }
            }
            return result;
        }

        [HttpGet("{name}/{group}")]
        public async Task<HrsTrigger> Get([FromRoute] string name, string group)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            var trigger = await scheduler.GetTrigger(new TriggerKey(name, group));
            if(trigger != null)
            {
                ICronTrigger cronTrigger = trigger as ICronTrigger;
                return new HrsTrigger
                {
                    SchedulerName = schedulerName,
                    TriggerName = name,
                    TriggerGroup = group,
                    JobName = trigger.JobKey.Name,
                    JobGroup = trigger.JobKey.Group,
                    Description = trigger.Description,
                    CronExpression = cronTrigger.CronExpressionString,
                    TimeZoneId = cronTrigger.TimeZone.Id
                };
            }
            return default;
        }

        [HttpPost]
        public async Task Post([FromBody] HrsTrigger hrsTrigger)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var triggerName = Guid.NewGuid();
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);

            var job = await scheduler.GetJobDetail(new JobKey(hrsTrigger.JobName, hrsTrigger.JobGroup));
            
            var trigger = TriggerBuilder.Create()
                .WithCronSchedule(hrsTrigger.CronExpression)
                .WithDescription(hrsTrigger.Description)
                .WithIdentity(triggerName.ToString(), hrsTrigger.TriggerGroup)
                .ForJob(job)
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(trigger);
        }

        [HttpPut("{name}/{group}")]
        public async Task Put(string name, string group, [FromBody] HrsTrigger hrsTrigger)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            var job = await scheduler.GetJobDetail(new JobKey(hrsTrigger.JobName, hrsTrigger.JobGroup));
            var oldTriggerKey = new TriggerKey(name, group);
            var trigger = await scheduler.GetTrigger(oldTriggerKey);
            var newTrigger = trigger.GetTriggerBuilder().WithDescription(hrsTrigger.Description).WithCronSchedule(hrsTrigger.CronExpression).ForJob(job).StartNow().Build();
            await scheduler.RescheduleJob(oldTriggerKey, newTrigger);
        }

        [HttpDelete("{name}/{group}")]
        public async Task Delete(string name, string group)
        {
            string schedulerName = GetSchedulerName(_Configuration.GetSection("Quartz"));
            var scheduler = await _SchedulerFactory.GetScheduler(schedulerName);
            await scheduler.UnscheduleJob(new TriggerKey(name, group));
        }
    }
}
