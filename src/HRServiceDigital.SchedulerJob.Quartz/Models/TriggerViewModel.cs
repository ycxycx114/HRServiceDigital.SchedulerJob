using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz.Models
{
    public class TriggerViewModel
    {
        public string SchedulerName { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; } = "DEFAULT";
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string Description { get; set; }
        public string TriggerState { get; set; }
        public string TriggerType { get; set; }
        public string CronExpression { get; set; }
        public string TimeZoneId { get; set; }

        public class TriggerViewModelMap : EntityMap<TriggerViewModel>
        {
            public TriggerViewModelMap()
            {
                Map(p => p.SchedulerName).ToColumn("SCHED_NAME");
                Map(p => p.TriggerName).ToColumn("TRIGGER_NAME");
                Map(p => p.TriggerGroup).ToColumn("TRIGGER_GROUP");
                Map(p => p.JobName).ToColumn("JOB_NAME");
                Map(p => p.JobGroup).ToColumn("JOB_GROUP");
                Map(p => p.Description).ToColumn("DESCRIPTION");
                Map(p => p.TriggerState).ToColumn("TRIGGER_STATE");
                Map(p => p.TriggerType).ToColumn("TRIGGER_TYPE");
                Map(p => p.CronExpression).ToColumn("CRON_EXPRESSION");
                Map(p => p.TimeZoneId).ToColumn("TIME_ZONE_ID");
            }
        }
    }
}
