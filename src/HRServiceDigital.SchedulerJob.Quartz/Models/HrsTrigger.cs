using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HRServiceDigital.SchedulerJob.Quartz.Models
{
    public class HrsTrigger
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
    }
}
